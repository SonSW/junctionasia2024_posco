import json
import subprocess
import threading
import time as time_module

from flask import Flask, render_template, request
from flask_cors import CORS
from flask_socketio import SocketIO, emit
from geopy.distance import geodesic

from coords import get_coordinates, get_estimated_time_ms
from waypoint import Waypoint  # Import the Waypoint class

# Define multiple bus routes with waypoints
waypoint_lists = [
    ["127.1058342, 37.3597078", "129.0759853, 35.1794697"],
    ["126.9783881, 37.5666103", "129.0759853, 35.1794697"],
    ["128.881561, 37.754693", "129.0759853, 35.1794697"]
]
number_of_taxis = len(waypoint_lists)


# Modify get_coordinates to return Waypoint objects
def get_coordinates_with_waypoints(start, end, waypoints):
    coordinates = get_coordinates(start, end, waypoints)
    return [Waypoint(lat, lon) for lat, lon in coordinates]


bus_routes = [get_coordinates_with_waypoints(waypoint[0], waypoint[-1], waypoint[1:-1]) for waypoint in waypoint_lists]

draw_preview = True
preview_route = get_coordinates_with_waypoints(
    "127.1058342, 37.3597078", "129.361465, 36.017393", ["129.0759853, 35.1794697"])

app = Flask(__name__)
CORS(app)
socketio = SocketIO(app, cors_allowed_origins="*")

# Global variable to store the current positions of the buses
current_positions = [route[0] for route in bus_routes]


# Calculate total distances for each bus
def calculate_total_distances(routes):
    return [sum(geodesic((route[i].lat, route[i].lon), (route[i + 1].lat, route[i + 1].lon)).meters for i in
                range(len(route) - 1)) for route in routes]


total_distances = calculate_total_distances(bus_routes)
max_distance = max(total_distances)

# Define a constant speed (e.g., 10 meters per second)
speed = 10000  # meters per second


# Calculate time intervals based on the constant speed
def calculate_time_intervals(routes):
    intervals = []
    for route in routes:
        segment_intervals = []
        for i in range(len(route) - 1):
            segment_distance = geodesic((route[i].lat, route[i].lon), (route[i + 1].lat, route[i + 1].lon)).meters
            segment_time = segment_distance / speed
            segment_intervals.append(segment_time)
        intervals.append(segment_intervals)
    return intervals


time_intervals = calculate_time_intervals(bus_routes)

# Initial zoom level
initial_zoom = 12

current_route_ixs = [0] * len(bus_routes)
important_bus_index = 0  # Default important bus is the first one


def simulate_bus_movement(bus_index):
    global current_positions
    global current_route_ixs
    while True:
        route = bus_routes[bus_index]
        i = current_route_ixs[bus_index]
        current_positions[bus_index] = route[i]
        socketio.emit('bus_position', {'bus_id': bus_index, 'lat': current_positions[bus_index].lat,
                                       'lon': current_positions[bus_index].lon})
        if i < len(time_intervals[bus_index]):
            time_module.sleep(time_intervals[bus_index][i])

        current_route_ixs[bus_index] = (i + 1) % len(route)


@app.route('/')
def index():
    return render_template('index.html')


@app.route('/map')
def map():
    return render_template('map.html', initial_zoom=initial_zoom, bus_routes=bus_routes,
                           important_bus_index=important_bus_index, preview_route=preview_route,
                           draw_preview=draw_preview)


@app.route('/ok_click', methods=['POST'])
def ok_click():
    return {'status': 'success'}


@socketio.on('connect')
def handle_connect():
    for bus_index, position in enumerate(current_positions):
        emit('bus_position', {'bus_id': bus_index, 'lat': position.lat, 'lon': position.lon})
    emit('important_bus_update', {'important_bus_index': important_bus_index, 'draw_preview': draw_preview})


@socketio.on('unity_message')
def handle_unity_message(data):
    print(f"debug: data = {data}")
    # Parse the JSON data
    data = json.loads(data)
    lat1 = data['lat1']
    lng1 = data['lng1']
    lat2 = data['lat2']
    lng2 = data['lng2']

    stations = []
    taxi_ix_by_station_ix = []

    for i, waypoint_list in enumerate(waypoint_lists):
        stations.extend(waypoint_list)
        taxi_ix_by_station_ix.extend([i] * len(waypoint_list))
    existing_count = len(stations)

    stations.append(f"{lng1}, {lat1}")
    stations.append(f"{lng2}, {lat2}")
    taxi_ix_by_station_ix.extend([-1] * 2)

    stations.extend([f"{wp.lon}, {wp.lat}" for wp in current_positions])
    taxi_ix_by_station_ix.extend([-1] * number_of_taxis)

    costs = [[0] * len(stations) for _ in range(len(stations))]
    for i in range(len(stations)):
        for j in range(len(stations)):
            if i == j:
                costs[i][j] = 1e5
            elif i >= len(stations) - number_of_taxis - 2 or j >= len(stations) - number_of_taxis - 2 or \
                    taxi_ix_by_station_ix[i] == taxi_ix_by_station_ix[j]:
                costs[i][j] = get_estimated_time_ms(stations[i], stations[j])
            else:
                costs[i][j] = 1e5
    cpp_input = ""
    cpp_input += f"{len(stations)}\n"
    cpp_input += "0 1\n"
    for i in range(len(stations)):
        for j in range(len(stations)):
            cpp_input += f"{i} {j} {costs[i][j]}\n"
    cpp_input += f"{number_of_taxis}\n"

    pos = 0
    for i in range(number_of_taxis):
        cpp_input += f"{len(waypoint_lists[i]) - 1}\n"
        for j in range(len(waypoint_lists[i]) - 1):
            cpp_input += f"{pos} {pos + 1}"
            pos += 1
        pos += 1

    result = subprocess.run(['./path_find'], input=cpp_input, capture_output=True, text=True)
    output = result.stdout.strip()

    sp = output.split()
    important_bus_index = int(sp[0])
    optimal_travel = []
    for out in list(map(int, sp[1:])):
        if out < existing_count:
            lon, lat = stations[out].split(", ")
            optimal_travel.append(Waypoint(lat, lon))
        elif out == existing_count:
            optimal_travel.append(Waypoint(lat1, lng1))
        elif out == existing_count + 1:
            optimal_travel.append(Waypoint(lat2, lng2))
        else:
            optimal_travel.append(current_positions[out - existing_count - 2])

    bus_routes[important_bus_index] = get_coordinates_with_waypoints(optimal_travel[0], optimal_travel[-1],
                                                                     optimal_travel[1:-1])
    current_route_ixs[important_bus_index] = 0
    current_positions[important_bus_index] = bus_routes[important_bus_index][0]

    # Emit the important_bus_update event
    socketio.emit('important_bus_update', {'important_bus_index': important_bus_index, 'draw_preview': draw_preview})


@socketio.on('unity_test_message')
def handle_unity_test_message(data):
    print(f"debug from unity: {data}!!!!!!")


if __name__ == '__main__':
    bus_threads = []
    for bus_index in range(len(bus_routes)):
        bus_thread = threading.Thread(target=simulate_bus_movement, args=(bus_index,))
        bus_thread.daemon = True
        bus_threads.append(bus_thread)
        bus_thread.start()
    socketio.run(app, host='0.0.0.0', port=5000, debug=True, allow_unsafe_werkzeug=True)

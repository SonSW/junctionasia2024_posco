import requests
from dotenv import load_dotenv
import os

# load .env
load_dotenv()


def get_coordinates(start, goal, waypoints=[]):
    client_id = os.environ.get("NAVER_CLIENT_ID")
    client_secret = os.environ.get("NAVER_CLIENT_SECRET")

    url = "https://naveropenapi.apigw.ntruss.com/map-direction-15/v1/driving"
    headers = {
        "X-NCP-APIGW-API-KEY-ID": client_id,
        "X-NCP-APIGW-API-KEY": client_secret
    }
    if waypoints:
        params = {"start": start, "goal": goal, "waypoints": "|".join(waypoints)}
    else:
        params = {"start": start, "goal": goal}
    response = requests.get(url, headers=headers, params=params)

    if response.status_code == 200:
        data = response.json()
        coordinates = []
        for lng, lat in data['route']['traoptimal'][0]['path']:
            coordinates.append([lat, lng])
    else:
        raise Exception(f"Failed to get data from Naver API: {response.status_code}")

    return coordinates


def get_estimated_time_ms(start, goal, waypoints=[]):
    client_id = os.environ.get("NAVER_CLIENT_ID")
    client_secret = os.environ.get("NAVER_CLIENT_SECRET")

    url = "https://naveropenapi.apigw.ntruss.com/map-direction-15/v1/driving"
    headers = {
        "X-NCP-APIGW-API-KEY-ID": client_id,
        "X-NCP-APIGW-API-KEY": client_secret
    }
    if waypoints:
        params = {"start": start, "goal": goal, "waypoints": "|".join(waypoints)}
    else:
        params = {"start": start, "goal": goal}
    response = requests.get(url, headers=headers, params=params)

    if response.status_code == 200:
        data = response.json()
        return data['route']['traoptimal'][0]['summary']['duration']
    else:
        raise Exception(f"Failed to get data from Naver API: {response.status_code}")

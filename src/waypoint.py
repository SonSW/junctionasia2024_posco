import uuid


class Waypoint:
    def __init__(self, lat, lon):
        self.lat = lat
        self.lon = lon
        self.id = uuid.uuid4()

    def __repr__(self):
        return f"Waypoint(lat={self.lat}, lon={self.lon}, id={self.id})"
class Pad:
    def __init__(self, id, node, status="available"):
        self.id = id
        self.node = node
        self.status = status

    def is_available(self):
        return self.status == "available"

    def occupy(self):
        if self.is_available():
            self.status = "occupied"

    def release(self):
        self.status = "available"
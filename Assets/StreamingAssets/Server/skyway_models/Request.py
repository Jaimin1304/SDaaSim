class Request:
    def __init__(self, packages, start_node, target_node, time_window):
        self.packages = packages
        self.start_node = start_node
        self.target_node = target_node
        self.time_window = time_window
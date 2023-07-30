import api_layer as api

def run():
    skyway = api.get_skyway()
    swarms = skyway['swarms']
    nodes = skyway['nodes']
    api.set_subswarm_targetNode('23c01419-cd69-48d7-a21d-dae173fe2b89', 'ff4757a6-ff00-42f7-a130-792517620228')

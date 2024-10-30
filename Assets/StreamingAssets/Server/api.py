import skyway_model as model


def set_subswarm_edge(subswarm_id, edge_id, instructions):
    instructions.append(
        {"set_subswarm_edge": {"subswarm_id": subswarm_id, "edge_id": edge_id}}
    )


def subswarm_hover(subswarm_id, instructions):
    instructions.append(
        {"set_subswarm_edge": {"subswarm_id": subswarm_id, "edge_id": ""}}
    )


def subswarm_land(subswarm_id, node_id, instructions):
    instructions.append(
        {"subswarm_land": {"subswarm_id": subswarm_id, "node_id": node_id}}
    )


def subswarm_land_at_curr_node(subswarm, instructions):
    if subswarm.edge != None:
        print(f"{subswarm.name} is in the middle of an edge")
        return
    instructions.append(
        {"subswarm_land": {"subswarm_id": subswarm.id, "node_id": subswarm.node.id}}
    )


def subswarm_takeoff(subswarm_id, node_id, instructions):
    instructions.append(
        {"subswarm_takeoff": {"subswarm_id": subswarm_id, "node_id": node_id}}
    )


def split_subswarm(subswarm_id, drone_lst, instructions):
    instructions.append(
        {
            "split_subswarm": {
                "subswarm_id": subswarm_id,
                "drone_lst": drone_lst,
                "edge_id": "",
            }
        }
    )


def split_subswarm(subswarm_id, drone_lst, edge_id, instructions):
    instructions.append(
        {
            "split_subswarm": {
                "subswarm_id": subswarm_id,
                "drone_lst": drone_lst,
                "edge_id": edge_id,
            }
        }
    )


def merge_two_subswarms(subswarmA, subswarmB, edge, instructions):
    instructions.append(
        {
            "merge_two_subswarms": {
                "subswarmA_id": subswarmA.id,
                "subswarmB_id": subswarmB.id,
                "edge_id": edge.id,
            }
        }
    )


def merge_all_subswarms_at_node(swarm, node, edge, instructions):
    instructions.append(
        {
            "merge_all_subswarms_at_node": {
                "swarm_id": swarm.id,
                "node_id": node.id,
                "edge_id": edge.id,
            }
        }
    )

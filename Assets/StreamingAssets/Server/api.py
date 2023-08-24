def set_subswarm_edge(subswarm_id, edge_id, instructions):
    instructions.append(
        {"set_subswarm_edge": {'subswarm_id': subswarm_id, 'edge_id': edge_id}})


def subswarm_land(subswarm_id, node_id, instructions):
    instructions.append(
        {"subswarm_land": {'subswarm_id': subswarm_id, 'node_id': node_id}})


def split_subswarm(subswarm_id, drone_lst, instructions):
    instructions.append(
        {"split_subswarm": {'subswarm_id': subswarm_id, 'drone_lst': drone_lst}})


def merge_two_subswarms(subswarmA_id, subswarmB_id, instructions):
    instructions.append(
        {"merge_two_subswarms": {
            'subswarmA_id': subswarmA_id, 'subswarmB_id': subswarmB_id}})

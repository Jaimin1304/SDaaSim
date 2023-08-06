def set_subswarm_edge(subswarm_id, edge_id, instructions):
    instructions.append({"set_subswarm_edge": {'subswarm_id': subswarm_id, 'edge_id': edge_id}})

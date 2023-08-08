from api import set_subswarm_edge
from skyway_model import Skyway
import random


def init():
    print('init your own data structures')


def run(skyway: Skyway):
    instructions = []

    print('run user logic')
    for subSwarm in skyway.subSwarms.values():
        random_edge_id = random.choice(list(subSwarm.node.edges.keys()))
        print('hey')
        set_subswarm_edge(subSwarm.id, random_edge_id, instructions)
    print('user logic execution complete')

    return instructions

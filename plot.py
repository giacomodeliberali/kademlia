import networkx as nx
import matplotlib.pyplot as plt
import statistics
import os
from operator import itemgetter as i
from functools import cmp_to_key

# START utility functions
def cmp(a, b):
    return (a > b) - (a < b) 

def multikeysort(items, columns):
    comparers = [
        ((i(col[1:].strip()), -1) if col.startswith('-') else (i(col.strip()), 1))
        for col in columns
    ]
    def comparer(left, right):
        comparer_iter = (
            cmp(fn(left), fn(right)) * mult
            for fn, mult in comparers
        )
        return next((result for result in comparer_iter if result), 0)
    return sorted(items, key=cmp_to_key(comparer))
# END utility functions

# input dir
stats_dir = "./stats"

stats_file = os.path.join(stats_dir, "stats.csv")
with open(stats_file, "r") as out_stats:
    lines = out_stats.readlines()
    skipped_first = False 

    graphs = []

    for line in lines:

        if not skipped_first: # skip first file
            skipped_first = True
            continue

        # parse values
        content = line.split(",")
        n = int(content[0])
        m = int(content[1])
        k = int(content[2])            
        deg = float(content[3])
        avg_clustering = float(content[4])
        diameter = float(content[5])
        average_shortest_path_length = float(content[6])

        # append as a dict
        graphs.append({
            'n': n,
            'm': m,
            'k': k,
            'deg': deg,
            'avg_clustering': avg_clustering,
            'diameter': diameter,
            'average_shortest_path_length': average_shortest_path_length
        })

    # sort by k then n
    graphs = multikeysort(graphs, ['k','n']) 

    # for each value of m
    for m_run in [9,10,64,160]:

        # initialize a new plot data set
        plots = {
            'k1': {
                'degrees': [],
                'nodes': [],
                'diameter': [],
                'average_shortest_path_length': [],
                'avg_clustering': []
            },
            'k5': {
                'degrees': [],
                'nodes': [],
                'diameter': [],
                'average_shortest_path_length': [],
                'avg_clustering': []
            },
            'k20': {
                'degrees': [],
                'nodes': [],
                'diameter': [],
                'average_shortest_path_length': [],
                'avg_clustering': []
            }
        }

        # for each network
        for g in graphs:

            k = g['k']
            n = g['n']
            m = g['m']
            deg = g['deg']
            diameter = g['diameter']
            avg_clustering = g['avg_clustering']
            average_shortest_path_length = g['average_shortest_path_length']

            # aggregate data with k and m as keys
            if m == m_run: 
                plots[f'k{k}']["degrees"].append(deg)
                plots[f'k{k}']["nodes"].append(n)
                plots[f'k{k}']["diameter"].append(diameter)
                plots[f'k{k}']["avg_clustering"].append(avg_clustering)
                plots[f'k{k}']["average_shortest_path_length"].append(average_shortest_path_length)

        # degree plot
        for k in [1,5,20]:
            plots_path = os.path.join("plots", f"avg_degree_m{m_run}.png")
            plt.plot(plots[f"k{k}"]["nodes"],plots[f"k{k}"]["degrees"])
            plt.xlabel('Nodes count')
            plt.ylabel('Degree')
            plt.title(f'm = {m_run}')
            plt.gca().legend(['k=1','k=5','k=20'],loc='center right')
            if k == 20: # generate only the last image with the 3 results 
                plt.savefig(plots_path, bbox_inches='tight')
                plt.clf()
            
        plt.clf()

        # average_shortest_path_length plot
        for k in [1,5,20]:
            plots_path = os.path.join("plots", f"pathlength_m{m_run}.png")
            plt.plot(plots[f"k{k}"]["nodes"],plots[f"k{k}"]["average_shortest_path_length"])
            plt.xlabel('Nodes count')
            plt.ylabel('Path length')
            plt.title(f'm = {m_run}')
            plt.gca().legend(['k=1','k=5','k=20'],loc="lower right")
            if k == 20: # generate only the last image with the 3 results
                plt.savefig(plots_path, bbox_inches='tight')
                plt.clf()
        
        plt.clf()

        # avg_clustering plot
        for k in [1,5,20]:
            plots_path = os.path.join("plots", f"avg_clustering_m{m_run}.png")
            plt.plot(plots[f"k{k}"]["nodes"],plots[f"k{k}"]["avg_clustering"])
            plt.xlabel('Nodes count')
            plt.ylabel('Clustering coefficient')
            plt.title(f'm = {m_run}')
            plt.gca().legend([f'k=1','k=5','k=20'])
            if k == 20: # generate only the last image with the 3 results
                plt.savefig(plots_path, bbox_inches='tight')
        plt.clf()


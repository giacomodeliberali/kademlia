import networkx as nx
import matplotlib.pyplot as plt
import statistics
import os

# input networks dir
stats_dir = "./stats"

# output 
stats = {}

# the list of the networks to analyse
files = os.listdir(stats_dir)

# write stats.csv header
out_stats_file = os.path.join(stats_dir, "stats.csv")
with open(out_stats_file, "w+") as out_stats:
    out_stats.write('n;m;k;degree;avg_clustering;diameter;average_path_length\n')

# display current progress
index = 1

for filename in files:
    if filename.startswith("graph_"):

        print(f"Analizing {filename} ({index}/{len(files)})...")
        index += 1

        # take params from file name (eg => graph_n1000_m160_k20_alpha3-2.csv)
        args = filename.split("_")
        n = args[1][1:]
        m = args[2][1:]
        k = args[3][1:]

        # parse the net
        input_graph = os.path.join(stats_dir, filename)
        G = nx.read_edgelist(input_graph, create_using=nx.DiGraph, delimiter=";")

        # build a key for this triplet
        key = f"n{n}m{m}k{k}"

        if key not in stats:
            stats[key] = {}

        stats[key]["n"] = n
        stats[key]["m"] = m
        stats[key]["k"] = k

        print(f"\t - Computing avg degree...")
        # calculate mean degree
        deg = round(statistics.mean(map(lambda f: int(f[1]), G.degree())), 3)
        if "deg" in stats[key]: # if a value for this triplet exist, make an avg
            stats[key]["deg"] = round(statistics.mean([stats[key]["deg"],deg]),3)
        else: # else this is the first value
            stats[key]["deg"] = deg

        print(f"\t - Computing avg clustering coefficient...")
        avg_clustering = round(nx.average_clustering(G), 3)
        if "avg_clustering" in stats[key]:
            stats[key]["avg_clustering"] = round(statistics.mean([stats[key]["avg_clustering"],avg_clustering]), 3)
        else:
            stats[key]["avg_clustering"] = avg_clustering

        print(f"\t - Computing avg path length...")
        average_shortest_path_length = round(nx.average_shortest_path_length(G), 2)
        if "average_shortest_path_length" in stats[key]:
            stats[key]["average_shortest_path_length"] = round(statistics.mean([stats[key]["average_shortest_path_length"],average_shortest_path_length]), 2)
        else:
            stats[key]["average_shortest_path_length"] = average_shortest_path_length

        print(f"\t - Computing diameter...")
        try:
            diameter = nx.diameter(G)

            if "diameter" in stats[key] and stats[key]["diameter"] != 0:
                stats[key]["diameter"] = round(statistics.mean([stats[key]["diameter"],diameter]), 2)
            else:
                stats[key]["diameter"] = diameter
        except: # nx.diameter(G) randomly throws an exception, what to to?
            if not "diameter" in stats[key]:
                stats[key]["diameter"] = 0

# write aggregated CSV report
out_stats_file = os.path.join(stats_dir, "stats.csv")
with open(out_stats_file, "a") as out_stats:
    for key in stats:
        out_stats.write(f'{stats[key]["n"]};{stats[key]["m"]};{stats[key]["k"]};{stats[key]["deg"]};{stats[key]["avg_clustering"]};{stats[key]["diameter"]};{stats[key]["average_shortest_path_length"]};\n')
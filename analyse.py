import networkx as nx
import matplotlib.pyplot as plt
import statistics
import os

stats_dir = "./stats"

stats = {}

files = os.listdir(stats_dir)

out_stats_file = os.path.join(stats_dir, "stats.csv")
with open(out_stats_file, "w+") as out_stats:
    out_stats.write('n;m;k;degree;avg_clustering;diameter;average_path_length\n')

index = 1

for filename in files:
    if filename.startswith("graph_"):

        print(f"Analizing {filename} ({index}/{len(files)})...")
        index += 1

        args = filename.split("_")
        n = args[1][1:]
        m = args[2][1:]
        k = args[3][1:]

        input_graph = os.path.join(stats_dir, filename)
        G = nx.read_edgelist(input_graph, create_using=nx.DiGraph, delimiter=";")

        key = f"n{n}m{m}k{k}"

        if key not in stats:
            stats[key] = {}

        stats[key]["n"] = n
        stats[key]["m"] = m
        stats[key]["k"] = k

        print(f"\t - Computing avg degree...")
            
        deg = round(statistics.mean(map(lambda f: int(f[1]), G.degree())), 3)
        if "deg" in stats[key]:
            stats[key]["deg"] = round(statistics.mean([stats[key]["deg"],deg]),3)
        else:
            stats[key]["deg"] = deg

        print(f"\t - Computing avg clustering coefficient...")
        avg_clustering = round(nx.average_clustering(G), 3)
        if "avg_clustering" in stats[key]:
            stats[key]["avg_clustering"] = round(statistics.mean([stats[key]["avg_clustering"],avg_clustering]), 3)
        else:
            stats[key]["avg_clustering"] = avg_clustering

        print(f"\t - Computing avg path length...")
        stats[key]["average_shortest_path_length"] = round(nx.average_shortest_path_length(G), 2)

        print(f"\t - Computing diameter...")
        try:
            diameter = nx.diameter(G)
            if diameter == 0:
                continue

            if "diameter" in stats[key] and stats[key]["diameter"] != 0:
                stats[key]["diameter"] = round(statistics.mean([stats[key]["diameter"],diameter]), 2)
            else:
                stats[key]["diameter"] = diameter
        except:
            if not "diameter" in stats[key]:
                stats[key]["diameter"] = 0

out_stats_file = os.path.join(stats_dir, "stats.csv")
with open(out_stats_file, "a") as out_stats:
    for key in stats:
        out_stats.write(f'{stats[key]["n"]};{stats[key]["m"]};{stats[key]["k"]};{stats[key]["deg"]};{stats[key]["avg_clustering"]};{stats[key]["diameter"]};{stats[key]["average_shortest_path_length"]};\n')
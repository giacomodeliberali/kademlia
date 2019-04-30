import matplotlib.pyplot as plt
import os

stats_file = os.path.join("test", "diameter.csv")
with open(stats_file, "r") as out_stats:
    lines = out_stats.readlines()

    nvalues = []
    diametervalues = []

    for line in lines:
        content = line.split(";")

        n = int(content[0])    
        diameter = int(content[1])  

        nvalues.append(n)  
        diametervalues.append(diameter)

    plots_path = os.path.join("plots", f"diameter_m160_k20.png")
    plt.plot(nvalues,diametervalues)
    plt.xlabel('Nodes count')
    plt.ylabel('Network diameter')
    #plt.title('Network diameter')
    plt.savefig(plots_path, bbox_inches='tight')
    plt.clf()  
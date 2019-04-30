#!/bin/bash
for k in 1 5 20
do
    for n in 100 200 300 400 500 600 700 800 900 1000
    do
        for m in 9 10 64 160
        do
            for i in 1 2 3
            do
                ./kademlia.sh $n $m $k
            done

        done
    done
done   
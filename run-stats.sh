#!/bin/bash
for k in 1 5 20
do
    for n in 10 100 250 1000 2000 5000
    do
        for m in 16 64 160
        do
            for i in 1 2
            do
                ./kademlia.sh $n $m $k &
            done

        done
    done
done   
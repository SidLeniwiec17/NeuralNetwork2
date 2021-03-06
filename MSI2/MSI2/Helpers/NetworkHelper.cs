﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MSI2.Content
{
    public class NetworkHelper
    {
        public static Network GenerateNetwork(Network network)
        {   
            if(network.CompleteData == true)
            {
                network.CompleteData = false;
                try
                {
                    Random rand = new Random(network.Seed);
                    Layer[] layers = new Layer[network.HiddenLayers + 1];

                    for (int i = 0; i < network.HiddenLayers + 1; i++)
                    {
                        Tuple<int, int> neurons = GetInAndOutNumber(network, i);
                        int input = neurons.Item1;
                        int output = neurons.Item2;
                        if (network.Bias)
                            input++;

                        layers[i] = new Layer(input, output);

                        for (int y = 0; y < input; y++)
                        {
                            for (int x = 0; x < output; x++)
                            {
                                float value = NextFloat(rand);
                                layers[i].Values[x, y] = value;
                            }
                        }
                    }
                    network.Layers = layers;
                    network.CompleteData = true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error ! " + ex.Message);
                }
            }
            return network;
        }

        public static float NextFloat(Random rnd)
        {
            //float min = 0.0f;
            //float max = 1.0f;
            float answ = -1.0f;
            float min = -0.4f;
            float max = 0.4f;
            while (answ < min || answ > max)
            {
                var tmpRnd = (float)rnd.NextDouble();
                answ = min + (tmpRnd * (max - min));
            }

            return answ;
        }

        public static Tuple<int, int> GetInAndOutNumber(Network network, int i)
        {
            int input = 0;
            int output = 0;
            if (i == 0)
            {
                input = network.InputLength;
                output = network.NeuronPerLayer[0];
            }
            else if (i == network.HiddenLayers)
            {
                input = network.NeuronPerLayer[network.NeuronPerLayer.Length - 1];
                output = network.Classes;
            }
            else
            {
                input = network.NeuronPerLayer[i - 1];
                output = network.NeuronPerLayer[i];
            }
            return new Tuple<int, int>(input, output);
        }
    }
}

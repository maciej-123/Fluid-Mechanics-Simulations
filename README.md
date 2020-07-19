# Fluid-Mechanics-Simulations

App to simulate Poiseuille flow.

            Poiseuille Flow Simulation
            1 - To use the simulation, please input a number for the 5 variables (R, L, Δp ,μ ,ρ) on the left.
            2 - The app will then calculate and display 4 variables on the right and display two graphs traces to show the shear and velocity flow profile.
            Please note that the graph scaling may vary for the shear and velocity profiles and the graph is not intended for reading off accurate values
            (these are given by the 4 variables on the right)
            3 - A box on the bottom right will display the powers of 10 of the shear and velocity profiles as a reference for the graph.
            4 - The application also determines whether the conditions for a poiseulle (laminar flow, L >> R) flow are met. The app will display warnings and/or black out calculated variables if these conditions are not met.\n\n" +
            5 - The non slip condition can be toggled on and off using 2 buttons found below the graph. Turing the no-slip conditon off will blackout the shear display box and adjust the graph accordingly. 
            Extras:
            Equations used to calculate variables:
            Re = 2 * ρ * V * R / μ
            V = (1 / (4 * μ)) * (Δp / L) * (R * R)
            τ = (Δp / L) * (R / 2)
            Q = (Pi * R^4 * Δp / (8 * μ * L))
            About the app: 水 is a Chinese pictogram meaning water/liquid.
            
Created by Maciej Zajaczkowski, copyright ©. (Feel free to use/modify this project - just attribute me in any derivative works)

Method for drawing graphs, Credit: https://www.codeproject.com/Articles/104820/Drawing-Trig-Functions-via-WPF

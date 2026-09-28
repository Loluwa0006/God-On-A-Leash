Temiloluwa Awosiyan
100977745

God On A Leash

God on A Leash is a breakneck pace physics action boss rush game where you’re tasked with fighting world ending lovecraftian sea deities armed with the world’s most hyperactive fishing rod and a disabled disgraced ship captain. Whip around an endless sea making your own level through grapple points for your magic dragon grappling hook as you bob and weave through an obstacle course of deadly projectiles looking for a decisive slash aided by various naval support abilities.

<img width="1755" height="642" alt="Ship Manager Code Diagram" src="https://github.com/user-attachments/assets/7cdf5797-cc0a-4707-8084-9f147ef4fe04" />

The ship ability adopts the factory pattern because in the final version of the game. 

This is because the amount of ship abilities as well as what type of ship ability the player has selected is dynamic, and created at run time. Additionally, having the generation be separate from any other class helps to keep the code clean by ensuring scripts only have one job. Using the factory pattern prevents other scripts like the player controller from having to make the ship ability instances themselves.


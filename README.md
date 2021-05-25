# Whales and Whalers: A Multi-agent System

## 1. Introduction 
Recent studies suggest that nineteenth-century sperm whales learned defensive behaviours against whalers through social learning. 

In this work, we modeled a similar problem using a multi-agent system in [Unity](https://unity.com/), featuring both whales and whalers as agents. The objective of the whalers is to capture all the whales as efficiently as possible, while the whales try to avoid being captured, for as long as possible. In order to accomplish this, the whales can communicate between themselves, cooperating to evade the ships, while the whalers can either cooperate or compete in order to successfully capture the biggest number of whales.

To learn more about this project you can read our [paper](/Project_Paper.pdf)

## 2. Usage

### 2.1 Loading the Project

The project was developed using [Unity 2020.3.0f1](https://unity3d.com/pt/unity/whats-new/2020.3.0), and we recommend using this version, although newer versions should work as well. To load the project, after installing [Unity Hub](https://unity3d.com/pt/get-unity/download) and choosing the Unity version you want to use, to import the project you just need to do the following:

    Projects -> ADD -> Project's root folder -> click on the new added project to load it

To open the source code, you can just right click on the ```scripts folder``` on the Project window, and select ```Open C# Project```.

![source code](/Demo/open_source_code.png)

Or you can just open the ```assets/scripts/``` folder on your preferred IDE.

### 2.2 Running the Project on Unity

We recommend running the project on Unity. After opening the project, to run in, you only need to press the play button on top of the window. In the **Game tab** you can see the project run like in the executables, with no debug information. If you open the **Scene tab** you can observe the wall sensors represented by red rays, and the vision sensor is represented by green rays.

![scene view](/Demo/scene_view.png)

On Unity it is also possible to train the whale's agents using TensorFlow, to do so you need to do the following steps:

1. Create a virtual environment: 
        
        python3 -m venv /path/to/new/virtual/environment

2. Activate the environment:

        source venv/bin/activate

3. Install the pip requirements:

        pip install -r requirements.txt

4. To run the script to train the agents:

        mlagents-learn config/trainer_config.yaml --run-id=FishTrain --resume 

5. Select the Fish Train scene and click the play button on Unity to start training
6. To observe the results:
        
        tensorboard --logdir=summaries

To deactive the virtual environment, you can use the command ```deactivate```. 

### 2.3 Opening Built Project

The build files for the project are located in the ```/Build``` directory, where you can find the executables for the following platforms:

* [Windows](/Builds/AASMA_Win_x86_x64.exe) (both x86 and x64 in ```/Builds/AASMA_Win_x86_x64.exe```)
* [Linux](/Builds/AASMA_Linux_x86_x64) (both x86 and x64 in ```/Builds/AASMA_Linux_x86_x64```)
* [Mac OS](/Builds/AASMA_MacOS_Intel_M1.app) (optimized for both Intel and Apple Silicon in ```/Build/AASMA_MacOS_Intel_M1.app```)

The app does not provide a close button, so you need to force close it yourself:

* In Windows and Linux ```alt+F4```
* In Mac OS ```cmd+Q```
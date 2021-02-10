# Smart Codesign VR
**A Smart Object Prototyping Platform in Virtual Reality for Children with NDD**

=> Demo video on YouTube [here](https://www.youtube.com/watch?v=iU2oaV-T-zE) (as of February 9, 2021)

<figure class="half" style="display:flex">
    <img style="width:100px" src="https://github.com/takafumihoriuchi/SmartCodesignVR/raw/develop/README_images/demoscreeshot1.png">
    <img style="width:100px" src="https://github.com/takafumihoriuchi/SmartCodesignVR/raw/develop/README_images/demoscreeshot2.png">
    <figcaption>(screenshot of play scene)</figcaption>
</figure>

<figure>
  <img src="https://github.com/takafumihoriuchi/SmartCodesignVR/raw/develop/README_images/figures4to6.png">
</figure>

## Abstract
Virtual reality is an emerging technology that has the potential of enhancing the learning experience of children. In this project, we designed and implemented Smart Codesign VR, a virtual reality platform for prototyping and interacting with smart-objects. The application is intended to be used in sequel to *The Smart Nature Protagonists (SNaP)* board-game, where children become the protagonists of the design process and collaborate with other players to ideate a concrete concept of a smart-object. Our application functions to virtually visualize the embodiment of the ideated smart-object. While designing the platform to satisfy the needs of a wide range of children, we paid special attention to children with neurodevelopmental disorders (NDD), in response to the fact that the SNaP board-game can also be used for children with special needs as therapeutic purposes. The implemented platform brings the benefits of allowing the players to experience the prototyping process using movements of their whole body and to visually percept their ideated smart-objects.


## Introduction
Smart Codesign VR is about the implementation of Virtual Reality (VR) technology in the experience of the *The Smart Nature Protagonist (SNaP)* board game. VR aims to enhance the overall experience while giving children a new level of immersion and understanding, with the ultimate goal of supporting the treatment of NDD children with the help of VR.

This VR application is intended to be a sequel or extention of the SNaP board game. The main idea of the game is to create a smart object using a combination of Input, Output and Environment cards that will solve the mission at the beginning of the game by the mayor. The game is played by 2 to 4 children (designers) and one moderator (mayor). The main goal of the SNaP board game is to serve as an educational tool that nurtures the creativity and social skills of children.

The research team has already developed a way to physically prototype the smart object designed by the children during the game. This is done through a physical board based on a tablet and an Arduino microcontroller. Smart Codesign VR comes in contrast with the physical prototyping solution by bringing in VR as a new medium for creation of and interaction with the smart objects. Virtual reality is an approach that the research team has not attempted yet.

The report is structured as follows: First, we will define the target users of the system and highlight their needs and constraints. Following that, we will investigate the state of the art around the topic of virtual reality for educational and therapeutic use, as well as the programming paradigm of visual programming enhanced by the application of virtual reality. We will then introduce our solution in designing an effective system of a smart object prototyping platform in virtual reality targeting children, especially those with neurodevelopmental disorders. We will discuss the value brought by our system, and mention the possible future work that can be done in order to put additional academic value to this project.


...


## UX Design
The concrete user experience that was implemented in the application will be described in this section. When the user launches the application on the Quest 2, they are first shown the Title “stand-by” screen (Figure 4). The purpose of this screen is to make sure that the next scene, the card selection scene, does not start until the user is fully ready with the VR equipment. Next, the user is directed to the card selection scene, where they select their choice of environment, input, and output cards (Figure 5). This is operated through grabbing the card representational objects, and dragging it into the corresponding box. The UX here is intentionally designed to be a “grab and drag” rather than a simple “selection by click”, in order to benefit from the 3D space provided in VR. When it is detected that all the three selection boxes contain an element, the scene transits to the “Prototyping Scene” with a voice guiding the users into the world of Smart Codesign VR. In the prototyping scene, the user is to define the condition and behavior of each input and output card (Figure 6). The UX here is designed for each card specifically, in order to gain the full benefit of 6DOF tracking VR technology. One of the key function here is that multiple input and output card statement sets can be defined in parallel, enabling, for example, a combination such as “When it is sunny, light up the LED in red;  when it is rainy, light up the LED in blue, and when it is snowing, light up the LED in white.”. This was an improvement that was made during the UX development phase, where the initial development plan was to allow only one set of input and output relations (e.g. “When it is sunny, light up the LED in red”). When the user defines the input and output statements, confirms the smart object, tests the behaviors, and finalizes the object, the user is directed to the “virtual park”, where they can interact with the smart object that they had prototyped (Figure 7).

<figure>
  <img src="https://github.com/takafumihoriuchi/SmartCodesignVR/raw/develop/README_images/figures4to6.png">
</figure>

<figure>
  <img src="https://github.com/takafumihoriuchi/SmartCodesignVR/raw/develop/README_images/figure7.png">
</figure>

The flow of activity that the user follows in the system is shown in the UML Activity Diagram in Figure 8. Here, the “Overall Activity” diagram on the top (in lateral page orientation) shows the overview of the actions that the user follows, and the diagrams on the bottom shows the user’s actions for specific card selection.

In the Overall Activity diagram, the user starts in the stand-by screen and transits to the next scene by pressing a button on the controller. Then, the user selects the environment card, input card, and the output card in an asynchronous manner (i.e. the order does not matter but all actions must be carried out). When the three cards are selected, the user gets instruction about the prototyping process in general as well as the specific interaction methods of each card selected. Here, the user has the freedom to go back to the card selection phase from the menu UI, in order to allow users to recover from mistakes in card selection. Then the user proceeds to the defining of the input condition statement and the output behavior statement. The details of these processes are described in the card specific activity diagrams. After defining the statement values of the input and output statements, the user confirms the smart object, tests the behavior of the prototyped smart object, and decides whether to finalize the smart object or re-edit the input/output statement values. Then the user is transferred to the virtual park environment with the prototyped smart object to interact with their object. The activity process ends when the user has activated the smart object in the park.

The diagram on the bottom-left, labeled “Define input condition statement : Weather card”, is a close-up activity diagram of the action node “defined input condition statement” in the Overall Activity diagram, in the case when “Weather” was selected as the input card. In this activity, the user grabs one of the weather representing objects, and irradiates the ray coming out from that object to the target environment object. The statement value is defined by this action, and the user can either proceed to the next action node, or to re-edit the statement definition.

In the similar manner, the diagram on the bottom-right, labeled “Define output behavior statement : Light Up card”, is a close-up diagram of the action node “define output behavior statement” in the Overall Activity diagram, in the case when “Light Up” was selected as the output card. In this activity, the user first grabs the paint brush object, then dips the brush tip into one of the paint buckets representing different colors. Then the user rubs the brush tip against the environment object and sets the color-value to the statement. After this, the user has the choice of adding another color to different parts of the environment object, dipping the brush in the bucket representing “water” and rubbing it against the colored part of the environment object to remove the applied color from the object, or proceeding to the next action.

<figure>
  <img src="https://github.com/takafumihoriuchi/SmartCodesignVR/raw/develop/README_images/figure8.png" style="width:100%">
  <figcaption align="center"><b>Figure 8.</b> UML Activity Diagram of the User’s Action</figcaption>
</figure>


## Implementation
Smart Codesign VR is a smart-object prototyping platform in virtual space. This requires the use of a VR headset as the hardware and a game framework for the creation of the software. To implement this system, we used Oculus Quest 2 as the operating hardware and Unity as the game framework.

### Hardware - Target Device
The target headset device is Oculus Quest 2. A differential feature of the Quest is its 6DOF tracking, which offers intuitive movements along with its two controllers (e.g. rotate, walk, grab, etc.). Although the Quest is yet to be widespread and accessible as the smartphone-based VR devices (i.e. 3DOF tracking devices), we believe that the 6DOF tracking feature is a crucial factor when designing a system for virtually prototyping smart-objects, which requires consistent interaction and manipulation of virtual objects.

### Software - Tools and Architecture

#### Tools
Smart Codesign VR was implemented on Unity as the game framework and C# as the programming language. For the interface between Unity and Oculus Quest, a package Oculus Integration was used, which offers a preset control of VR camera behavior, renderings, and an unified input API for controllers.

#### Architecture
The project consists of thirteen classes (among which three are abstract classes), as shown in the UML class diagram in Figure 9. Note that the attributes and the methods of the classes are simplified for improving the comprehensiveness of the diagram. The project consists of four packages (referred to as “scenes” in Unity): Title Scene, Card Selection Scene, Prototyping Scene, and Interaction Scene. 

The Title Scene, with a class “TitleSceneCore”, shows a standby screen, where the user transits to the Card Selection Scene upon the press of a controller button.

The Card Selection Scene, with classes “CardSelectionSceneCore” and “CardSelectionDetector”, is where the user selects their three SNaP cards: environment object card, input card, and output card. The CardSelectionSceneCore class creates three instances of the CardSelectionDetector class, each representing the functions of selection-boxes for environment object, input, and output, and links the box GameObjects to these functions (i.e. links object instances to function instances). Each instantiated box inherits the function to drag the card representation objects inside the box when the user brings them near the box, and marks the category as “selected”. When the CardSelectionSceneCore detects that all three categories of cards are selected, it sends the selection data to the “SmartObject” class to record the selected cards, then transits the scene to the Prototyping Scene. Here, it is worth noting that the SmartObject class is a static class, thereby the stored data is preserved without being destroyed between scene transitions.

The Prototyping Scene constitutes the core functionality of the system, where it manages the prototyping of the smart object. The “PrototypingSceneCore” class is where all the procedural operation is managed in a centralized mannar. Other classes, the “Card” class, the “InputCard” class, the “OutputCard” class, and the specific card classes (e.g. “Weather” class, “LightUp” class, etc. ) exist for the purpose of defining the behaviour of the two selected input and output cards. The specific card classes can be grouped into subclasses of two superclasses: the “InputCard” class and the “OutputCard” class. These two superclasses can also be assigned to a superclass: the “Card” class. Classes that are relative to the prototyping action are the specific card classes, thus the “Card” class and the “InputCard” and “OutputCard” classes are marked as abstract classes.

The prototyping procedure is managed as follows: The card selection data is retrieved from the SmartObject class, which was set during the previous scene. The PrototypingSceneCore class holds a list of input card condition instances and another list of output card behavior instances. The elements in each of the lists are all instances of the same specific class, that is, if the selected cards are “weather” card and “light up” card, the input card condition instance list consists of one or more instances of the “Weather” class, and the output card behavior instance list consists of one or more instances of the “LightUp” class. In the beginning of the procedure, the PrototypingSceneCore class automatically generates one instance from the specific input card class and the specific output card class, and adds them to the list. Then, players can press the “add instance button” in the UI to generate more instances to add to the list. This “multi-instancing” feature is the distinctive feature of the final Smart Codesign VR system in comparison to the early concept of the system, where it was originally planned to have one input instance and one output instance. In the “mono-instance” concept, the prototyping experience tended to be monotonous, where the smart object would only have one input-output relation (e.g. “When it is sunny, light up the LED in red”). In contrast to this, with the implementation of multi-instancing behavior, the smart object to be prototyped became more expressive, with multiple input-output relations (e.g. “When it is sunny, light up the LED in red;  when it is rainy, light up the LED in blue, and when it is snowing, light up the LED in white.”). In every frame, the PrototypingSceneCore class updates the input card condition of all the input instances in the list, and calls the output behavior assigned to the corresponding output instance. When the user confirms the prototype of the smart object, the boolean property “isConfirmed” is set to true, and the behavioral data of the prototyped smart object is set to the static properties of the SmartObject class. The process is then carried on to the Interaction Scene.

The Interaction Scene consists of the class “InteractionSceneCore”, where it manages the process where the user places their prototyped smart object in the virtual park, and executes the input condition and observes the output behavior. The class also contains a method to call the congratulations message that leads the player to the ending of the Smart Codesign VR experience.

<figure>
  <img src="https://github.com/takafumihoriuchi/SmartCodesignVR/raw/develop/README_images/figure9.png" style="width:100%">
  <figcaption align="center"><b>Figure 9.</b> UML Class Diagram of the System of Smart Codesign VR</figcaption>
</figure>

## Value Proposition
Used along with the SNaP board game, we believe that the implemented system has the potential for serving as a new way of helping children acquire the skills of creativity and expressiveness. This point is based on three major reasonings: enhanced visual representation, introduction of physical gestures, and the activity outcome formed in a digital data format.

Firstly, the use of VR technology to occlude the field of view and enhancing the vision brings the value of offering the users an immersive prototyping experience. It has been shown by research that these characteristics of VR can alleviate the symptoms of the sensory integration dysfunction that characterizes persons with NDD. In the 6DOF virtual platform, the users can observe the smart object of interest from all directions, in an identical manner as they would in the real world. Although this serves as a strong characteristic of the application of the VR technology, it is worth noting the other aspect of VR, that immersivity may increase the risk of self-isolation, with the child sticking to the virtual imaginary world and unwilling to return to the “real” world”. This problem is assumed to be avoided in our current implementation, as there is a specific ending to the activity, which concludes when the prototyping and the interaction phase is over. To be sure on this point, it is desirable for an empirical study to be carried out.

Secondly, the UX of our system involves physical gestures, such as grabbing their chosen cards and placing them inside a floating box, directing the rays of lights at the target object, speaking into the virtual microphone to record their voice, and using a virtual paintbrush to attach colors to the smart objects. These gestures are known to have positive effects in education for children. The use of physical gestures are known not only to enhance the user’s ability to connect abstract to concrete, but also to improve their memory and cognitive skills, such as strategic and spatial cognition and the reasoning skills used in problem solvings. In addition, the motor skills involved in the gesturing actions works to support the retention of the learned concepts, since these motions build links that act as cues to represent and recall the acquired knowledge. Although the current implementation of our system is limited to the conditional statements (“if ... then ...” statements) for programming the logical structures, further increasing the types of available programming semantics would benefit the players learning the fundamentals of programming and to acquire the basic computational thinking skills.

Finally, our system brings the value of generating the prototyping result in the form of digital data. Generally speaking, the SNaP board game ends with a written down sentence of the smart object that the players had ideated during the game play. The “physical board” (i.e. a system consisting of a tablet and an Arduino for making a physical representation of the ideated smart object), currently developed in the i3Lab, adds to this result values brought by its “tangible” characteristics. Although this could bring numbers of benefits for the children’s learning process, one shortage point is that the total experience of the SNaP board game is limited within the workshop session, and ends when the workshop is over. This limits the children to become the protagonists of their design only during the session, and not any longer after the workshop session. On the contrary, by using Smart Codesign VR and making the prototype of the smart object in a digital format, children can share their ideas to the online community, and continue to be the protagonists of their design not only during the workshop session, but also after the workshop when ever they have access to the Internet.

...
   M S B A M L   `   `   `  ����   BodyBasics-WPF4    .Microsoft.Samples.Kinect.BodyBasics.MainWindow   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     -     � Title$  Body Basics��$	��768��$
��1024�� � 	Resources    MediumGreyBrush �� Color  KinectPurpleBrush  KinectBlueBrush��%]   &	        &	      &	 *     �� $ 	#ff6e6e6e���� $ 	#ff52318f���� $ 	#ff00BCF2��.��� $��	10 0 10 0q� � RowDefinitions �� $
��Auto ��� $��* ��� $
��Auto �.+�(� .���� $
��None=� J� Opacity$	 0.5Q����� *AwarenessImageSource+(� $��1��$��Center=�.���� $��UniformToFill=����� *ColorImageSource+(� $��1��$��Center=�.���� $��UniformToFill=����� *ImageSource+(� $��1��$��Center=�.���� -    ��Name$ 
DepthImage��$��UniformToFill=����� *DepthImageSource+ ��  �� ElementName$ depthSlider�� �� Path$ Value��� -   $��	statusBar��$��2��$��Stretch=�$��Bottom=�
�������#��[  $��0,0,410.333,-0.333q�.@��� $
��Left=����� *
StatusText+�� -    ��Name$ savingTo��$��Saving to: ��$��Right=��� -   	 ��Name$	 startRecording��$��Start Recording?��$
��Left=�$��378,4.333,0,0q�$	��Top=�$	��157��$��22��$��2��(� $
��Left=�$��31,10.333,0,-0.333q�$	��Top=�$	��157��$��22��$��2��
 (� Stretch$

 Fill=��� -    ��Name$ displayImage��$��Display Image��$
��Left=�$��560,9.5,0,0q�$��2��$	��Top=�$��17��$	��102�� d� 	IsChecked$
 TrueY��� -   $ displayBodies��$��Display Bodies (0)��$
��Left=�$��667,9.5,0,0q�$��2��$	��Top=�$��18��$	��126��$
 TrueY��� -   $ displayTalk��$��Display Talk��$
��Left=�$��798,9.333,0,0q�$��2��$	��Top=�$��18��$��90��
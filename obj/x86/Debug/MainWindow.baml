   M S B A M L   `   `   `  ����  BodyBasics-WPF4    .Microsoft.Samples.Kinect.BodyBasics.MainWindow   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   -   5          � Title$  Body Basics��5   	   $	��768��5   	   $
��1024��6    � 	Resources 5         MediumGreyBrush �� Color  KinectPurpleBrush  KinectBlueBrush��%�   &	        &	 #     &	 F     �� 5	   
   $ 	#ff6e6e6e��62   �� 5
   
   $ 	#ff52318f��64   �� 5   
   $ 	#ff00BCF2��62   5      .��5      � $��	10 0 10 0q�6    � RowDefinitions 5   
   �� 5      $
��Auto �6   �� 5      $��* �6   �� 5      $
��Auto �6   .+�5   
   �� $��0��6   $��Images\Logo.png��6   $
��Left=�66   $
��Fill=�6Q   $��32��6`   $��81��6l   $��0 10 0 5q�6w   �� 5   
   $��0��6   $��0,0,-0.667,0.333q�6!   $��Right=�6;   $��Bottom=�6W   #��[  6r   $��Segoe UI)�6�   $��18(�6�   $	��244��6�   $!��Kinect Data Collection Tool��6�   �� 5   
   $��0��6   $��Images\Status.png��6   $
��None=�68   $��Center=�6G   $��124,5,823.6,0.2q�6d   (� 5   
   $��1��6   $��Center=�6   .��5      �� $��UniformToFill=�68   ��6   �� *ColorImageSource+68   5      (� 5   
   $��1��6   $��Center=�6   .��5      �� $��UniformToFill=�63   ��6   �� *ImageSource+63   5      (� 5    
   $��1��6   $��Center=�6   .��5!      �� -   5         ��Name$ 
DepthImage��5!      $��UniformToFill=�6L   ��6(   �� *DepthImageSource+ J� Opacity 6d   ��  �� ElementName$ depthSlider�� �� Path$ Value�5"      �� 5$   
   -   5        $��	statusBar��5$   ?   $��2��6   $��Stretch=�6!   $��Bottom=�6P   
�������#��[  6~   .@�5%      �� $
��Left=�6;   ��6   �� *
StatusText+6;   �� 5&      -   5         ��Name$ savingTo��5&      $��Saving to: ��6&   $��Right=�6<   5'      �� 5(   
   -   5        	 ��Name$	 startRecording��5(      $��Start Recording?��6)   $
��Left=�6D   $��
221,11,0,0q�6_   $	��Top=�6s   $	��157��6�   $��35��6�   6�   �� 5)   
   -   5        
 ��Name$
 depthSlider��5)      $
��Left=�6&   $��
423,25,0,0q�6A   $	��Top=�6U   $��20��6m   $	��112��6y    � Maximum$ 1Q�6�   �� 5*   
   -   5        $ depthImagelabel��5*      $��Show Depth Image��6)   $
��Left=�6D   $��	423,5,0,0q�6_   $	��Top=�6r   5+      5,      
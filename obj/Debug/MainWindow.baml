   M S B A M L   `   `   `  ����E  AFusion_PDO, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'clr-namespace:Fusion_PDO
Fusion_PDO  5   	       Fusion_PDO.MainWindow   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   _ [MaterialDesignThemes.Wpf, Version=4.5.0.0, Culture=neutral, PublicKeyToken=df2a72020bd7962aFmaterialdesign1http://materialdesigninxaml.net/winfx/xaml/themes  5   	   $localclr-namespace:Fusion_PDO  5   	   -   5          � Title$*  $Fusion Production Document Organizer��5
   	   $	��700��66   $
��1300��6C   $	��800��6P   $	��500��6_    � WindowStartupLocation$ CenterScreen=�6o    � 
ResizeMode$ CanResizeWithGrip=�6�    � 	Resources 5      '  !MaterialDesignThemes.Wpf.PackIcon   Kind 	  close  open  	CloseMenu �� TargetProperty �� 
TargetName9  3System.Windows.Media.Animation.EasingDoubleKeyFrame O� KeyTime O� Value  OpenMenu��%�  &	        &	 0     &	 \     &	       5   
   $ HamburgerMenuBack=�6"   6;     5   
   $ HamburgerMenu=�6"   67   �� 5   
   .��5      U� $ (Grid.Width)�6,   $
 grid��6U   .\�5        $ 0��6'   $	 250Q�63     5      $ 0:0:0.4��6'   $ 0Q�69   5      5      �� 5   
   .��5      U� $ (Grid.Width)�6,   $
 grid��6U   .\�5        $ 0��6'   $ 0Q�63     5      $ 0:0:0.4��6'   $	 250Q�69   5      5      5      .��5      � 	 � ColumnDefinitions	 5   
   �� 5       $
��auto �6   �� 5!      $��* �6   .+�5%   
   � -   5        
 �Name$

 grid��5%      $	��250��6   ���	#FFBD7307 J� RenderTransformOrigin$ 0.5,0.5&�6@   $��0��6`   .+�5&      �� $��Disabled=�6   $
��Auto=�6D   $��0,23,0,0q�6g   $��2��6y   .��5'      �� -   5         ��Name$
 Menu��5'      5(      5*      � 5,   
   $��1��6   .+�5-      �� ��5.      �� 5/      .��51      �  � Color$ 	#FFEAAF42��6'    � Offset$	 0.0Q�69   � 52      $ 	#FF523811��6'   $ 1Q�69   54      56      �� 57      -   5         ��Name$ button��57      $
��Left=�6%   $	��Top=�6@   $��1q�6X      MaterialDesignFlatDarkButton#��[ 6c   
�������$��50��6�   .��58      C�  C� ResourceKey$ close��6"   59      � 5<      -   5        $
 StackPanelMain��5<      $��1��6+   $��Stretch=�6;   $��10,30,10,10q�6Y   5>      5?      5A      5B      
   M S B A M L   `   `   `  ���� N  JWordament Solver CV, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullM+clr-namespace:Wordament_Solver_CV.ViewModelWordament_Solver_CV.ViewModel  )    #Wordament_Solver_CV.View.GameScreen   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     @d2http://schemas.microsoft.com/expression/blend/2008     :cv+clr-namespace:Wordament_Solver_CV.ViewModel       N� Title$  
GameScreen�� � 	Resources    
boardStyle �� 
TargetType �� Property �� Value  Vm S� 
ObjectType/   )Wordament_Solver_CV.ViewModel.GameService��%�   &	        &	 x     �� " ��.���  � ��
 �������  � ��$ 34(���  � ��$ 5q���  �@��TextAlignment$ Center=�S� "  .��� $	��325��$��10q� � DataContext# [ .+�� $��Right=�$	��Top=�$��	0,50,10,0q�$	��240��$	��240�� � RowDefinitions �� �� �� ��  � ColumnDefinitions �� �� �� �� .+��� -   	 ��Name$	 	textBlock��#��[  
 J� RenderTransformOrigin$
 0.5,0.5&����� *CurrentTheme+���� *
TileString+  TileStringConvertors �� 	Converter# [  �� ConverterParameter$ 0���� -   $	 
textBlock4��#��[  $��1��$
 0.5,0.5&����� *CurrentTheme+���� *
TileString+# [ $ 1���� -   $	 
textBlock8��#��[  $��2��$
 0.5,0.5&����� *CurrentTheme+���� *
TileString+# [ $ 2���� -   $	 textBlock12��#��[  $��3��$
 0.5,0.5&����� *CurrentTheme+���� *
TileString+# [ $ 3���� -   $	 
textBlock1��#��[  $��1��$
 0.5,0.5&����� *CurrentTheme+���� *
TileString+# [ $ 4���� -   $	 
textBlock5��#��[  $��1��$��1��$
 0.5,0.5&����� *CurrentTheme+���� *
TileString+# [ $ 5���� -   $	 
textBlock9��#��[  $��1��$��2��$
 0.5,0.5&����� *CurrentTheme+���� *
TileString+# [ $ 6���� -   $	 textBlock13��#��[  $��1��$��3��$
 0.5,0.5&����� *CurrentTheme+���� *
TileString+# [ $ 7���� -	   $	 
textBlock2��#��[  $��2��$
 0.5,0.5&����� *CurrentTheme+���� *
TileString+# [ $ 8���� -
   $	 
textBlock6��#��[  $��2��$��1��$
 0.5,0.5&����� *CurrentTheme+���� *
TileString+# [ $ 9���� -   $	 textBlock10��#��[  $��2��$��2��$
 0.5,0.5&����� *CurrentTheme+���� *
TileString+# [ $ 10���� -   $	 textBlock14��#��[  $��2��$��3��$
 0.5,0.5&����� *CurrentTheme+���� *
TileString+# [ $ 11���� -   $	 
textBlock3��#��[  $��3��$
 0.5,0.5&����� *CurrentTheme+���� *
TileString+# [ $ 12���� -   $	 
textBlock7��#��[  $��3��$��1��$
 0.5,0.5&����� *CurrentTheme+���� *
TileString+# [ $ 13���� -   $	 textBlock11��#��[  $��3��$��2��$
 0.5,0.5&����� *CurrentTheme+���� *
TileString+# [ $ 14���� -   $	 textBlock15��#��[  $��3��$��3��$
 0.5,0.5&����� *CurrentTheme+���� *
TileString+# [ $ 15���� $
��Left=�$	��Top=�$��15.96������ *CurrentGameState+�� $��Center=�$	��Top=�$��14(����� *Status+�� $
��Wrap=�$	��Top=�$	��240��$	��240��$
��Left=�$��	10,50,0,0q����� *	Meaning+�� $	��150��$��Center=�$��Bottom=� � Minimum$ 1Q� � Maximum$ 50Q�$��25�� � Value �� *Speed+
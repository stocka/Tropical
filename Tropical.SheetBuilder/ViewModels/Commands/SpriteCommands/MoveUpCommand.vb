Imports Tropical.Models

''' <summary>
''' A command for moving up a sprite.
''' </summary>
Public Class MoveUpCommand
  Inherits MoveCommandBase

  ''' <summary>
  ''' Initializes a new instance of the <see cref="MoveUpCommand"/> class.
  ''' </summary>
  ''' <param name="service">The sprite sheet service.</param>
  ''' <param name="canExecute">The function to be invoked as necessary
  ''' by <see cref="CanExecute" />.</param>
  ''' <param name="movedUp">The method to invoke after a sprite
  ''' has been successfully moved. The affected sprite will be
  ''' passed to this method.</param>
  Public Sub New(service As SpriteSheetService,
                 canExecute As Func(Of Boolean),
                 movedUp As Action(Of Sprite))
    MyBase.New(service, canExecute, movedUp)
  End Sub
  
  ''' <summary>
  ''' Moves up the provided sprite.
  ''' </summary>
  ''' <param name="parameter">The sprite to move.</param>
  Public Overrides Sub Execute(parameter As Object)
    ' Move up.
    MyBase.ExecuteMoveBase(parameter, True)
  End Sub

End Class

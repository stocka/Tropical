Imports Tropical.Models

''' <summary>
''' A command for moving down a sprite.
''' </summary>
Public Class MoveDownCommand
  Inherits MoveCommandBase

  ''' <summary>
  ''' Initializes a new instance of the <see cref="MoveDownCommand"/> class.
  ''' </summary>
  ''' <param name="service">The sprite sheet service.</param>
  ''' <param name="canExecute">The function to be invoked as necessary
  ''' by <see cref="CanExecute" />.</param>
  ''' <param name="movingDown">The method to invoke before a sprite
  ''' will be moved.  The affected sprite will be
  ''' passed to this method.</param>
  ''' <param name="movedDown">The method to invoke after a sprite
  ''' has been successfully moved. The affected sprite will be
  ''' passed to this method.</param>
  Public Sub New(service As SpriteSheetService,
                 canExecute As Func(Of Boolean),
                 movingDown As Action(Of Sprite),
                 movedDown As Action(Of Sprite))
    MyBase.New(service, canExecute, movingDown, movedDown)
  End Sub

  ''' <summary>
  ''' Moves up the provided sprite.
  ''' </summary>
  ''' <param name="parameter">The sprite to move.</param>
  Public Overrides Sub Execute(parameter As Object)
    ' Move down.
    MyBase.ExecuteMoveBase(parameter, False)
  End Sub

End Class

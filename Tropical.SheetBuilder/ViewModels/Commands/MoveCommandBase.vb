Imports Tropical.Models

''' <summary>
''' A base class for moving a sprite up or down.
''' </summary>
Public MustInherit Class MoveCommandBase
  Inherits SpriteCommandBase

  ''' <summary>
  ''' Initializes a new instance of the <see cref="MoveCommandBase"/> class.
  ''' </summary>
  ''' <param name="service">The sprite sheet service.</param>
  ''' <param name="canExecute">The function to be invoked as necessary
  ''' by <see cref="CanExecute" />.</param>
  ''' <param name="moved">The method to invoke after a sprite
  ''' has been successfully moved. The affected sprite will be
  ''' passed to this method.</param>
  Public Sub New(service As SpriteSheetService,
                 canExecute As Func(Of Boolean),
                 moved As Action(Of Sprite))
    MyBase.New(service, canExecute, moved)
  End Sub

  ''' <summary>
  ''' Moves a sprite up or down.
  ''' </summary>
  ''' <param name="parameter">The sprite to move.</param>
  ''' <param name="moveUp">If set to <c>true</c>, will move the
  ''' sprite up by one; otherwise, will move it down by one.</param>
  Protected Sub ExecuteMoveBase(parameter As Object, moveUp As Boolean)

    If CanExecute(parameter) Then

      ' Try to cast this as a sprite, make sure it exists
      Dim sprite = TryCast(parameter, Sprite)

      If sprite IsNot Nothing Then

        ' Now try to move it, invoking the event handler as appropriate
        If _service.MoveSprite(sprite, moveUp) AndAlso _onExecuted IsNot Nothing Then
          _onExecuted(sprite)
        End If

      End If
    End If
  End Sub

End Class

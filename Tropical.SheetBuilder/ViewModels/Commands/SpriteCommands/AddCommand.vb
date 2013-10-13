Imports Tropical.Models

''' <summary>
''' A command for deleting a sprite.
''' </summary>
Public Class AddCommand
  Inherits SpriteCommandBase
  Implements ICommand

  ''' <summary>
  ''' Initializes a new instance of the <see cref="AddCommand"/> class.
  ''' </summary>
  ''' <param name="service">The sprite sheet service.</param>
  ''' <param name="canExecute">The function to be invoked as necessary
  ''' by <see cref="CanExecute" />.</param>
  ''' <param name="added">The method to invoke after a sprite
  ''' has been successfully deleted. The new sprite will be
  ''' passed to this method.</param>
  Public Sub New(service As SpriteSheetService,
                 canExecute As Func(Of Boolean),
                 added As Action(Of Sprite))
    MyBase.New(service, canExecute, added)
  End Sub

  ''' <summary>
  ''' Creates a new sprite.
  ''' </summary>
  ''' <param name="parameter">The sprite at which the new sprite will be placed.
  ''' If this value is <c>null</c>, the sprite will be placed at the end
  ''' of the list.</param>
  Public Overrides Sub Execute(parameter As Object)

    If CanExecute(parameter) Then

      ' Try to cast this as a sprite, but we don't require that
      ' it be defined.  We're just using it for adding at a
      ' specific position.
      Dim sprite = TryCast(parameter, Sprite)

      Dim createdSprite As Sprite = _service.AddSprite(sprite)

      ' If we added it successfully, and we have an event handler
      ' for addition, invoke it.
      If createdSprite IsNot Nothing Then
        MyBase.TryExecuteHandler(createdSprite)
      End If

    End If

  End Sub

End Class
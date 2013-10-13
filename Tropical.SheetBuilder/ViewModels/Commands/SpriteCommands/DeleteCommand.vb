Imports Tropical.Models

''' <summary>
''' A command for deleting a sprite.
''' </summary>
Public Class DeleteCommand
  Inherits SpriteCommandBase
  Implements ICommand

  ''' <summary>
  ''' Initializes a new instance of the <see cref="DeleteCommand"/> class.
  ''' </summary>
  ''' <param name="service">The sprite sheet service.</param>
  ''' <param name="canExecute">The function to be invoked as necessary
  ''' by <see cref="CanExecute" />.</param>
  ''' <param name="deleted">The method to invoke after a sprite
  ''' has been successfully deleted. The deleted sprite will be
  ''' passed to this method.</param>
  Public Sub New(service As SpriteSheetService,
                 canExecute As Func(Of Boolean),
                 deleted As Action(Of Sprite))
    MyBase.New(service, canExecute, deleted)
  End Sub

  ''' <summary>
  ''' Deletes the provided sprite.
  ''' </summary>
  ''' <param name="parameter">The sprite to delete.</param>
  Public Overrides Sub Execute(parameter As Object)

    If CanExecute(parameter) Then

      ' Make sure it's a sprite.
      Dim sprite = TryCast(parameter, Sprite)
      If sprite IsNot Nothing Then

        ' Show a confirmation dialog
        Dim result = MessageBox.Show("Are you sure you wish to delete the sprite?", "Confirm", MessageBoxButton.OKCancel)

        If result = MessageBoxResult.OK Then

          ' If we deleted it successfully, and we have an event handler
          ' for deletion, invoke it.
          If _service.DeleteSprite(sprite) Then
            MyBase.TryExecuteHandler(sprite)
          End If

        End If
      End If
    End If
  End Sub

End Class
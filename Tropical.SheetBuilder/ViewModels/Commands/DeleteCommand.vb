Imports Tropical.Models

Public Class DeleteCommand
  Implements ICommand

  Private ReadOnly _service As SpriteSheetService
  Private ReadOnly _canExecute As Func(Of Boolean)
  Private ReadOnly _deleted As Action(Of Sprite)

  Public Sub New(service As SpriteSheetService,
                 canExecute As Func(Of Boolean),
                 deleted As Action(Of Sprite))
    _service = service
    _canExecute = canExecute
    _deleted = deleted
  End Sub

  Public Function CanExecute(parameter As Object) As Boolean Implements ICommand.CanExecute
    Return _canExecute()
  End Function

  Public Sub Execute(parameter As Object) Implements ICommand.Execute

    If CanExecute(parameter) Then

      ' Make sure it's a sprite.
      Dim sprite = TryCast(parameter, Sprite)
      If sprite IsNot Nothing Then

        ' Show a confirmation dialog
        Dim result = MessageBox.Show("Are you sure you wish to delete the sprite?", "Confirm", MessageBoxButton.OKCancel)

        If result = MessageBoxResult.OK Then

          ' If we deleted it successfully, and we have an event handler
          ' for deletion, invoke it.
          If _service.DeleteSprite(sprite) AndAlso _deleted IsNot Nothing Then
            _deleted(sprite)
          End If
        End If
      End If
    End If
  End Sub

  Public Event CanExecuteChanged As EventHandler Implements ICommand.CanExecuteChanged

  Public Sub RaiseCanExecuteChanged()
    RaiseEvent CanExecuteChanged(Me, EventArgs.Empty)
  End Sub

End Class
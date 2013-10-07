Imports Tropical.Models

Public Class AddCommand
  Implements ICommand

  Private ReadOnly _service As SpriteSheetService
  Private ReadOnly _canExecute As Func(Of Boolean)
  Private ReadOnly _added As Action(Of Sprite)

  Public Sub New(service As SpriteSheetService,
                 canExecute As Func(Of Boolean),
                 added As Action(Of Sprite))
    _service = service
    _canExecute = canExecute
    _added = added
  End Sub

  Public Function CanExecute(parameter As Object) As Boolean Implements ICommand.CanExecute
    Return _canExecute()
  End Function

  Public Sub Execute(parameter As Object) Implements ICommand.Execute

    If CanExecute(parameter) Then

      ' Try to cast this as a sprite, but we don't require that
      ' it be defined.  We're just using it for adding at a
      ' specific position.
      Dim sprite = TryCast(parameter, Sprite)

      Dim createdSprite As Sprite = _service.AddSprite(sprite)

      ' If we added it successfully, and we have an event handler
      ' for addition, invoke it.
      If createdSprite IsNot Nothing AndAlso _added IsNot Nothing Then
        _added(sprite)
      End If

    End If

  End Sub

  Public Event CanExecuteChanged As EventHandler Implements ICommand.CanExecuteChanged

  Public Sub RaiseCanExecuteChanged()
    RaiseEvent CanExecuteChanged(Me, EventArgs.Empty)
  End Sub

End Class
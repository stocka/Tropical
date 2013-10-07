Imports Tropical.Models

Public MustInherit Class MoveCommandBase
  Implements ICommand

  Private ReadOnly _service As SpriteSheetService
  Private ReadOnly _canExecute As Func(Of Boolean)
  Private ReadOnly _moved As Action(Of Sprite)

  Public Sub New(service As SpriteSheetService,
                 canExecute As Func(Of Boolean),
                 moved As Action(Of Sprite))
    _service = service
    _canExecute = canExecute
    _moved = moved
  End Sub

  Public Function CanExecute(parameter As Object) As Boolean Implements ICommand.CanExecute
    Return _canExecute()
  End Function

  Public MustOverride Sub Execute(parameter As Object) Implements ICommand.Execute

  Protected Sub ExecuteMoveBase(parameter As Object, moveUp As Boolean)

    If CanExecute(parameter) Then

      ' Try to cast this as a sprite, make sure it exists
      Dim sprite = TryCast(parameter, Sprite)

      If sprite IsNot Nothing Then

        ' Now try to move it, invoking the event handler as appropriate
        If _service.MoveSprite(sprite, moveUp) AndAlso _moved IsNot Nothing Then
          _moved(sprite)
        End If

      End If

    End If

  End Sub

  Public Event CanExecuteChanged As EventHandler Implements ICommand.CanExecuteChanged

  Public Sub RaiseCanExecuteChanged()
    RaiseEvent CanExecuteChanged(Me, EventArgs.Empty)
  End Sub

End Class

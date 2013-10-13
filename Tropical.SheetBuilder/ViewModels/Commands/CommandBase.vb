''' <summary>
''' A base class for simplified implementation of
''' <see cref="ICommand" />.
''' </summary>
''' <typeparam name="T">The type of item that
''' will be provided to the successful execution
''' callback.</typeparam>
Public MustInherit Class CommandBase(Of T)
  Implements ICommand

  Protected ReadOnly _service As SpriteSheetService
  Protected ReadOnly _canExecute As Func(Of Boolean)
  Protected ReadOnly _onExecuted As Action(Of T)

  ''' <summary>
  ''' Initializes a new instance of the <see cref="CommandBase(Of T)"/> class.
  ''' </summary>
  ''' <param name="service">The sprite sheet service.</param>
  ''' <param name="canExecute">The function to be invoked as necessary
  ''' by <see cref="CanExecute" />.</param>
  ''' <param name="onExecuted">The method to invoke after a command
  ''' has been successfully executed. The affected sprite will be
  ''' passed to this method.</param>
  Public Sub New(service As SpriteSheetService,
                 canExecute As Func(Of Boolean),
                 onExecuted As Action(Of T))
    _service = service
    _canExecute = canExecute
    _onExecuted = onExecuted
  End Sub

  ''' <summary>
  ''' Defines the method that determines whether the command can execute in its current state.
  ''' </summary>
  ''' <param name="parameter">Data used by the command.  
  ''' If the command does not require data to be passed, this object can be set to <c>null</c>.</param>
  ''' <returns>
  ''' <c>true</c> if this command can be executed; otherwise, <c>false</c>.
  ''' </returns>
  Public Function CanExecute(parameter As Object) As Boolean Implements ICommand.CanExecute
    Return _canExecute()
  End Function

  ''' <summary>
  ''' Occurs when changes occur that affect whether or not the command should execute.
  ''' </summary>
  Public Event CanExecuteChanged(sender As Object, e As EventArgs) Implements ICommand.CanExecuteChanged

  ''' <summary>
  ''' Defines the method to be called when the command is invoked.
  ''' </summary>
  ''' <param name="parameter">Data used by the command.  
  ''' If the command does not require data to be passed, this object can be set to <c>null</c>.</param>
  Public MustOverride Sub Execute(parameter As Object) Implements ICommand.Execute

  ''' <summary>
  ''' Raises the <see cref="CanExecuteChanged" /> event.
  ''' </summary>
  Public Sub RaiseCanExecuteChanged()
    RaiseEvent CanExecuteChanged(Me, EventArgs.Empty)
  End Sub

  ''' <summary>
  ''' Attempts to invoke the <see cref="_onExecuted" /> handler
  ''' if it exists.
  ''' </summary>
  ''' <param name="handlerArgument">The data to pass to the handler.</param>
  Protected Sub TryExecuteHandler(handlerArgument As T)

    If _onExecuted IsNot Nothing Then
      Me._onExecuted(handlerArgument)
    End If

  End Sub

End Class

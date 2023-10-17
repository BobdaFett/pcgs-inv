using System;
using System.Reactive;
using System.Windows.Input;
using System.Xml;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using ReactiveUI;

namespace PcgsInvUi.ViewModels;

public class DeleteWindowViewModel : ViewModelBase
{
    public ReactiveCommand<Unit, object> OkCommand { get; } = ReactiveCommand.Create(() => (object)true);
    public ReactiveCommand<Unit, object> CancelCommand { get; } = ReactiveCommand.Create(() => (object)false);
}
using System.Reactive.Linq;
using ReactiveUI;
using WalletWasabi.Fluent.ViewModels.Dialogs.Base;

namespace WalletWasabi.Fluent.ViewModels.Dialogs
{
	[NavigationMetaData(Title = "Enter your password")]
	public partial class EnterPasswordDialogViewModel : DialogViewModelBase<string?>
	{
		[AutoNotify] private string? _password;

		public EnterPasswordDialogViewModel(string caption, bool enableEmpty = true)
		{
			Caption = caption;

			// This means pressing continue will make the password empty string.
			// pressing cancel will return null.
			_password = "";

			var backCommandCanExecute = this.WhenAnyValue(x => x.IsDialogOpen).ObserveOn(RxApp.MainThreadScheduler);

			var nextCommandCanExecute = this.WhenAnyValue(
					x => x.IsDialogOpen,
					x => x.Password,
					delegate
					{
						// This will fire validations before return canExecute value.
						this.RaisePropertyChanged(nameof(Password));

						return IsDialogOpen &&
						       ((enableEmpty && string.IsNullOrEmpty(Password)) ||
						        (!string.IsNullOrEmpty(Password) && !Validations.Any));
					})
				.ObserveOn(RxApp.MainThreadScheduler);

			var cancelCommandCanExecute = this.WhenAnyValue(x => x.IsDialogOpen).ObserveOn(RxApp.MainThreadScheduler);

			BackCommand = ReactiveCommand.Create(() => Close(DialogResultKind.Back), backCommandCanExecute);
			NextCommand = ReactiveCommand.Create(() => Close(result: Password), nextCommandCanExecute);
			CancelCommand = ReactiveCommand.Create(() => Close(DialogResultKind.Cancel), cancelCommandCanExecute);
		}

		public string Caption { get; }

		protected override void OnDialogClosed()
		{
			Password = "";
		}
	}
}

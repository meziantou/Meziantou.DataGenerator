using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using CodeFluent.Runtime.Database.Design;

namespace Meziantou.DataGenerator.Design
{
    public class ConnectionStringTextBox : TextBox
    {
        private ConnectionStringObject _connectionStringObject;

        public ConnectionStringObject ConnectionStringObject
        {
            get
            {
                return this._connectionStringObject;
            }
            set
            {
                if (this._connectionStringObject == value)
                    return;

                if (this._connectionStringObject != null)
                {
                    this._connectionStringObject.PropertyChanged -= this.OnConnectionStringObjectPropertyChanged;
                }

                this._connectionStringObject = value;
                if (this._connectionStringObject != null)
                {
                    this.Text = this._connectionStringObject.ToString();
                    this._connectionStringObject.PropertyChanged += this.OnConnectionStringObjectPropertyChanged;
                }
                else
                {
                    this.Text = string.Empty;
                }
            }
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            if (this.ConnectionStringObject != null && !string.IsNullOrEmpty(this.Text) && e.Key == Key.Return)
            {
                this.ConnectionStringObject.Reset(this.Text);
            }

            base.OnKeyUp(e);
        }

        protected override void OnLostFocus(RoutedEventArgs e)
        {
            if (this.ConnectionStringObject != null && !string.IsNullOrEmpty(this.Text))
            {
                this.ConnectionStringObject.Reset(this.Text);
            }

            base.OnLostFocus(e);
        }

        protected virtual void OnConnectionStringObjectPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            this.Text = this._connectionStringObject.ToString();
        }
    }
}

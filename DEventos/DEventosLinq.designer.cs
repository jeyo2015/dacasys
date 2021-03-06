﻿#pragma warning disable 1591
//------------------------------------------------------------------------------
// <auto-generated>
//     Este código fue generado por una herramienta.
//     Versión de runtime:4.0.30319.1008
//
//     Los cambios en este archivo podrían causar un comportamiento incorrecto y se perderán si
//     se vuelve a generar el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DEventos
{
	using System.Data.Linq;
	using System.Data.Linq.Mapping;
	using System.Data;
	using System.Collections.Generic;
	using System.Reflection;
	using System.Linq;
	using System.Linq.Expressions;
	using System.ComponentModel;
	using System;
	
	
	[global::System.Data.Linq.Mapping.DatabaseAttribute(Name="db_dentista")]
	public partial class DEventosLinqDataContext : System.Data.Linq.DataContext
	{
		
		private static System.Data.Linq.Mapping.MappingSource mappingSource = new AttributeMappingSource();
		
    #region Definiciones de métodos de extensibilidad
    partial void OnCreated();
    partial void InsertLogErrores(LogErrores instance);
    partial void UpdateLogErrores(LogErrores instance);
    partial void DeleteLogErrores(LogErrores instance);
    partial void InsertBitacora(Bitacora instance);
    partial void UpdateBitacora(Bitacora instance);
    partial void DeleteBitacora(Bitacora instance);
    partial void InsertParametroSistemas(ParametroSistemas instance);
    partial void UpdateParametroSistemas(ParametroSistemas instance);
    partial void DeleteParametroSistemas(ParametroSistemas instance);
    #endregion
		
		public DEventosLinqDataContext() : 
				base(global::DEventos.Properties.Settings.Default.db_dentistaConnectionString2, mappingSource)
		{
			OnCreated();
		}
		
		public DEventosLinqDataContext(string connection) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public DEventosLinqDataContext(System.Data.IDbConnection connection) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public DEventosLinqDataContext(string connection, System.Data.Linq.Mapping.MappingSource mappingSource) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public DEventosLinqDataContext(System.Data.IDbConnection connection, System.Data.Linq.Mapping.MappingSource mappingSource) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public System.Data.Linq.Table<LogErrores> LogErrores
		{
			get
			{
				return this.GetTable<LogErrores>();
			}
		}
		
		public System.Data.Linq.Table<Bitacora> Bitacora
		{
			get
			{
				return this.GetTable<Bitacora>();
			}
		}
		
		public System.Data.Linq.Table<ParametroSistemas> ParametroSistemas
		{
			get
			{
				return this.GetTable<ParametroSistemas>();
			}
		}
	}
	
	[global::System.Data.Linq.Mapping.TableAttribute(Name="dbo.LogErrores")]
	public partial class LogErrores : INotifyPropertyChanging, INotifyPropertyChanged
	{
		
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		private int _ID;
		
		private System.Nullable<System.DateTime> _FechaHora;
		
		private string _Descripcion;
		
    #region Definiciones de métodos de extensibilidad
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
    partial void OnIDChanging(int value);
    partial void OnIDChanged();
    partial void OnFechaHoraChanging(System.Nullable<System.DateTime> value);
    partial void OnFechaHoraChanged();
    partial void OnDescripcionChanging(string value);
    partial void OnDescripcionChanged();
    #endregion
		
		public LogErrores()
		{
			OnCreated();
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_ID", AutoSync=AutoSync.OnInsert, DbType="Int NOT NULL IDENTITY", IsPrimaryKey=true, IsDbGenerated=true)]
		public int ID
		{
			get
			{
				return this._ID;
			}
			set
			{
				if ((this._ID != value))
				{
					this.OnIDChanging(value);
					this.SendPropertyChanging();
					this._ID = value;
					this.SendPropertyChanged("ID");
					this.OnIDChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_FechaHora", DbType="DateTime")]
		public System.Nullable<System.DateTime> FechaHora
		{
			get
			{
				return this._FechaHora;
			}
			set
			{
				if ((this._FechaHora != value))
				{
					this.OnFechaHoraChanging(value);
					this.SendPropertyChanging();
					this._FechaHora = value;
					this.SendPropertyChanged("FechaHora");
					this.OnFechaHoraChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Descripcion", DbType="NVarChar(200)")]
		public string Descripcion
		{
			get
			{
				return this._Descripcion;
			}
			set
			{
				if ((this._Descripcion != value))
				{
					this.OnDescripcionChanging(value);
					this.SendPropertyChanging();
					this._Descripcion = value;
					this.SendPropertyChanged("Descripcion");
					this.OnDescripcionChanged();
				}
			}
		}
		
		public event PropertyChangingEventHandler PropertyChanging;
		
		public event PropertyChangedEventHandler PropertyChanged;
		
		protected virtual void SendPropertyChanging()
		{
			if ((this.PropertyChanging != null))
			{
				this.PropertyChanging(this, emptyChangingEventArgs);
			}
		}
		
		protected virtual void SendPropertyChanged(String propertyName)
		{
			if ((this.PropertyChanged != null))
			{
				this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
	}
	
	[global::System.Data.Linq.Mapping.TableAttribute(Name="dbo.Bitacora")]
	public partial class Bitacora : INotifyPropertyChanging, INotifyPropertyChanged
	{
		
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		private int _ID;
		
		private System.DateTime _FechaHora;
		
		private string _Descripcion;
		
		private string _IDUsuario;
		
    #region Definiciones de métodos de extensibilidad
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
    partial void OnIDChanging(int value);
    partial void OnIDChanged();
    partial void OnFechaHoraChanging(System.DateTime value);
    partial void OnFechaHoraChanged();
    partial void OnDescripcionChanging(string value);
    partial void OnDescripcionChanged();
    partial void OnIDUsuarioChanging(string value);
    partial void OnIDUsuarioChanged();
    #endregion
		
		public Bitacora()
		{
			OnCreated();
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_ID", AutoSync=AutoSync.OnInsert, DbType="Int NOT NULL IDENTITY", IsPrimaryKey=true, IsDbGenerated=true)]
		public int ID
		{
			get
			{
				return this._ID;
			}
			set
			{
				if ((this._ID != value))
				{
					this.OnIDChanging(value);
					this.SendPropertyChanging();
					this._ID = value;
					this.SendPropertyChanged("ID");
					this.OnIDChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_FechaHora", DbType="DateTime NOT NULL")]
		public System.DateTime FechaHora
		{
			get
			{
				return this._FechaHora;
			}
			set
			{
				if ((this._FechaHora != value))
				{
					this.OnFechaHoraChanging(value);
					this.SendPropertyChanging();
					this._FechaHora = value;
					this.SendPropertyChanged("FechaHora");
					this.OnFechaHoraChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Descripcion", DbType="NVarChar(100) NOT NULL", CanBeNull=false)]
		public string Descripcion
		{
			get
			{
				return this._Descripcion;
			}
			set
			{
				if ((this._Descripcion != value))
				{
					this.OnDescripcionChanging(value);
					this.SendPropertyChanging();
					this._Descripcion = value;
					this.SendPropertyChanged("Descripcion");
					this.OnDescripcionChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_IDUsuario", DbType="NVarChar(50) NOT NULL", CanBeNull=false)]
		public string IDUsuario
		{
			get
			{
				return this._IDUsuario;
			}
			set
			{
				if ((this._IDUsuario != value))
				{
					this.OnIDUsuarioChanging(value);
					this.SendPropertyChanging();
					this._IDUsuario = value;
					this.SendPropertyChanged("IDUsuario");
					this.OnIDUsuarioChanged();
				}
			}
		}
		
		public event PropertyChangingEventHandler PropertyChanging;
		
		public event PropertyChangedEventHandler PropertyChanged;
		
		protected virtual void SendPropertyChanging()
		{
			if ((this.PropertyChanging != null))
			{
				this.PropertyChanging(this, emptyChangingEventArgs);
			}
		}
		
		protected virtual void SendPropertyChanged(String propertyName)
		{
			if ((this.PropertyChanged != null))
			{
				this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
	}
	
	[global::System.Data.Linq.Mapping.TableAttribute(Name="dbo.ParametroSistemas")]
	public partial class ParametroSistemas : INotifyPropertyChanging, INotifyPropertyChanged
	{
		
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		private int _IDParametro;
		
		private string _Elemento;
		
		private string _ValorS;
		
		private System.Nullable<int> _ValorI;
		
    #region Definiciones de métodos de extensibilidad
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
    partial void OnIDParametroChanging(int value);
    partial void OnIDParametroChanged();
    partial void OnElementoChanging(string value);
    partial void OnElementoChanged();
    partial void OnValorSChanging(string value);
    partial void OnValorSChanged();
    partial void OnValorIChanging(System.Nullable<int> value);
    partial void OnValorIChanged();
    #endregion
		
		public ParametroSistemas()
		{
			OnCreated();
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_IDParametro", AutoSync=AutoSync.OnInsert, DbType="Int NOT NULL IDENTITY", IsPrimaryKey=true, IsDbGenerated=true)]
		public int IDParametro
		{
			get
			{
				return this._IDParametro;
			}
			set
			{
				if ((this._IDParametro != value))
				{
					this.OnIDParametroChanging(value);
					this.SendPropertyChanging();
					this._IDParametro = value;
					this.SendPropertyChanged("IDParametro");
					this.OnIDParametroChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Elemento", DbType="VarChar(30) NOT NULL", CanBeNull=false)]
		public string Elemento
		{
			get
			{
				return this._Elemento;
			}
			set
			{
				if ((this._Elemento != value))
				{
					this.OnElementoChanging(value);
					this.SendPropertyChanging();
					this._Elemento = value;
					this.SendPropertyChanged("Elemento");
					this.OnElementoChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_ValorS", DbType="VarChar(100)")]
		public string ValorS
		{
			get
			{
				return this._ValorS;
			}
			set
			{
				if ((this._ValorS != value))
				{
					this.OnValorSChanging(value);
					this.SendPropertyChanging();
					this._ValorS = value;
					this.SendPropertyChanged("ValorS");
					this.OnValorSChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_ValorI", DbType="Int")]
		public System.Nullable<int> ValorI
		{
			get
			{
				return this._ValorI;
			}
			set
			{
				if ((this._ValorI != value))
				{
					this.OnValorIChanging(value);
					this.SendPropertyChanging();
					this._ValorI = value;
					this.SendPropertyChanged("ValorI");
					this.OnValorIChanged();
				}
			}
		}
		
		public event PropertyChangingEventHandler PropertyChanging;
		
		public event PropertyChangedEventHandler PropertyChanged;
		
		protected virtual void SendPropertyChanging()
		{
			if ((this.PropertyChanging != null))
			{
				this.PropertyChanging(this, emptyChangingEventArgs);
			}
		}
		
		protected virtual void SendPropertyChanged(String propertyName)
		{
			if ((this.PropertyChanged != null))
			{
				this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
	}
}
#pragma warning restore 1591

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.Net;
using System.Threading;
using System.Net.Http;
using static WF_Exam.Сетевое_программирование.Checkers;
using System.IO;

namespace WF_Exam.Сетевое_программирование
{
    public partial class Checkers : Form
    {
        const int _mapSize = 8; // Размер доски
        const int _cellSize = 50; // Размер кнопок

        Image _white; // Изображения шашек белого игрока
        Image _black; // Изображения шашек черного игрока
        public string _pathWhiteFig = @"C:\Users\User\Desktop\Image\w.png"; // Путь к изображению белых шашек
        public string _pathBlackFig = @"C:\Users\User\Desktop\Image\b.png"; // Путь к изображению черных шашек

        public int[,] _map = new int[_mapSize, _mapSize]; // Массив, представляющий доску и расположение шашек

        public Button _prevBtn; // Предыдущая выбранная кнопка
        public Button _pressBtn; // Текущая выбранная кнопка
        public int _currentPlayer; // Текущий игрок (1 - белые, 2 - черные)
        public bool _isMoving; // Флаг, указывающий на то, что игрок совершает ход
        public bool _isServer; // Флаг, указывающий, что текущий игрок - сервер
        public bool _isStartGame; // Флаг, указывающий, что игра началась
        public bool _isStart_White; // Флаг, указывающий, что первыми ходят белые (1 - да, 0 - нет)

        public TcpListener _tcpServer = null; // Сокет для сервера
        public TcpClient _tcpClient = null; // Сокет для клиента
        public string _ipServer = "0.0.0.0"; // IP-адрес сервера
        public int _portServer = 12345; // Порт для соединения
        public MoveInfo _moveInfo = new MoveInfo();

        string _msg = "";
        byte[] _buffer = new byte[1024];
        enum Options { Error = 0, OK };

        public Checkers()
        {
            InitializeComponent();

            Init(); // Инициализация игры
        }

        // Инициализация игры
        public void Init()
        {
            ThreadPool.SetMinThreads(3, 3);
            ThreadPool.SetMaxThreads(10, 10);

            // Загрузка изображений шашек
            _white = new Bitmap(new Bitmap(_pathWhiteFig), new Size(_cellSize - 10, _cellSize - 10));
            _black = new Bitmap(new Bitmap(_pathBlackFig), new Size(_cellSize - 10, _cellSize - 10));

            // Подписываемся на события изменения выбора режима (сервер/клиент)
            rbIsServer.CheckedChanged += rbIsServer_Checked;
            rbIsClient.CheckedChanged += rbIsClient_Checked;

            _currentPlayer = 1; // Устанавливаем текущего игрока (1 - белые)
            _isMoving = false; // Устанавливаем флаг, что игрок не совершает ход
            _prevBtn = null; // Сбрасываем предыдущую выбранную кнопку

            // Инициализация массива, представляющего доску и расположение шашек
            _map = new int[_mapSize, _mapSize]
            {
                { 0, 2, 0, 2, 0, 2, 0, 2 },
                { 2, 0, 2, 0, 2, 0, 2, 0 },
                { 0, 2, 0, 2, 0, 2, 0, 2 },
                { 0, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0, 0, 0 },
                { 1, 0, 1, 0, 1, 0, 1, 0 },
                { 0, 1, 0, 1, 0, 1, 0, 1 },
                { 1, 0, 1, 0, 1, 0, 1, 0 },
            };

            CreateMap(); // Создаем доску
        }

        // Создание доски и расстановка шашек
        public void CreateMap()
        {
            this.Width = (_mapSize + 10) * _cellSize;
            this.Height = (_mapSize + 1) * _cellSize;

            for (int i = 0; i < _mapSize; i++)
            {
                for (int j = 0; j < _mapSize; j++)
                {
                    Button _btn = new Button();
                    _btn.Location = new Point(j * _cellSize, i * _cellSize);
                    _btn.Size = new Size(_cellSize, _cellSize);
                    _btn.Click += new EventHandler(OnFigurePress);

                    if (_map[i, j] == 1) _btn.Image = _white; // Расставляем белые шашки
                    if (_map[i, j] == 2) _btn.Image = _black; // Расставляем черные шашки

                    _btn.BackColor = GetPrevButtonColor(_btn);

                    this.Controls.Add(_btn);
                }
            }
        }


        private void btnStartGame_Click(object sender, EventArgs e)
        {
            try
            {
                if (_isServer)
                {
                    rbIsServer.Enabled = true;
                    rbIsClient.Enabled = false;
                    _isStart_White = true;
                    FirstPlayer(_isStart_White);


                    ThreadPool.QueueUserWorkItem(ServerProc, this);

                }
                else if (!_isServer)
                {
                    rbIsServer.Enabled = false;
                    rbIsClient.Enabled = true;
                    _isStart_White = true;
                    FirstPlayer(_isStart_White);

                    ThreadPool.QueueUserWorkItem(ClientProc, this);
                }

                Init();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "ERROR (btnStartGame_Click)");
                AddToJournal($"ERROR (btnStartGame_Click) = {ex.Message} \r\n");
            }
        }


        // поток сервера
        private void ServerProc(object obj)
        {
            Checkers form = obj as Checkers;
            //Options op;
            try
            {
                _tcpServer = new TcpListener(IPAddress.Parse(_ipServer), _portServer);
                _tcpServer.Start();

                while (true)
                {
                    // Запуск сервера
                    TcpClient _client = _tcpServer.AcceptTcpClient();
                    if (_tcpClient != null)
                    {
                        _msg = _msg = "ERROR: Client is already conected!";
                        NetworkStream ns = _client.GetStream();
                        byte[] _buf = Encoding.UTF8.GetBytes(_msg);
                        ns.Write(_buf, 0, _buf.Length);
                        _client.Close();
                    }
                    else
                    {
                        _tcpClient = _client;
                        //_msg = Options.OK.ToString();
                        _msg = "OK";
                        byte[] _buf = Encoding.UTF8.GetBytes(_msg);
                        _tcpClient.GetStream().Write(_buf, 0, _buf.Length);
                        ThreadPool.QueueUserWorkItem(ReceiveOrSend, this);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "ERROR (ServerProc)");
                AddToJournal($"ERROR (ServerProc) - {ex.Message}\r\n");
            }
        }

        private void ClientProc(object obj)
        {
            Checkers form = obj as Checkers;

            if (_tcpClient != null)
                return; // ERROR - этот клиент уже подключен к серверу

            _tcpClient = new TcpClient();
            try
            {
                // подключение к серверу игры
                _ipServer = "127.0.0.1"; // локальное подключение
                _tcpClient.Connect(_ipServer, _portServer);
                if (_tcpClient.Connected)
                {
                    NetworkStream ns = _tcpClient.GetStream();
                    int size = ns.Read(_buffer, 0, _buffer.Length);
                    _msg = Encoding.UTF8.GetString(_buffer, 0, size);
                    if (_msg == "OK")
                    {
                        ThreadPool.QueueUserWorkItem(ReceiveOrSend, this);
                        AddToJournal($"INFO(ClientProc) - Клиент подключился к Серверу успешно\r\n");
                    }
                    else
                    {
                        MessageBox.Show("Отказ от сервера", "ERROR (ClientProc)");
                        AddToJournal($"ERROR (ClientProc) - \"Отказ от сервера\" {_msg} \r\n");
                        ClosingTcp();
                        _tcpClient = null;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "ERROR (ClientProc)");
                AddToJournal($"ERROR (ClientProc) - {ex.Message}\r\n");
            }
        }

        private void ReceiveOrSend(object obj)
        {
            Checkers form = obj as Checkers;
            try
            {
                form._tcpClient.Client.BeginReceive(form._buffer, 0, form._buffer.Length, SocketFlags.None, CallReceive, form);

                while (form._tcpClient.Connected)
                {
                    NetworkStream ns = _tcpClient.GetStream();
                    string moveInfo = $"{_moveInfo._pressPoint.X},{_moveInfo._pressPoint.Y},{_moveInfo._prevPoint.X},{_moveInfo._prevPoint.Y}";
                    byte[] _buf = Encoding.UTF8.GetBytes(moveInfo);
                    ns.WriteAsync(_buf, 0, _buf.Length);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "ERROR (ReceiveOrSend)");
                AddToJournal($"ERROR (ReceiveOrSend) - {ex.Message}\r\n");
            }
            finally
            {
                ClosingTcp();
                _tcpClient = null;
            }
        }


        private void CallReceive(IAsyncResult ar)
        {
            if (ar.IsCompleted)
            {
                Checkers form = ar.AsyncState as Checkers;
                try
                {
                    int size = form._tcpClient.Client.EndReceive(ar);
                    _msg = Encoding.UTF8.GetString(form._buffer, 0, size);
                    form.Invoke((Action)(() =>
                    {
                        string[] temp = _msg.Split(',');

                        MoveInfo tempMove = new MoveInfo();
                        tempMove._pressPoint.X = int.Parse(temp[0]);
                        tempMove._pressPoint.Y = int.Parse(temp[1]);
                        tempMove._prevPoint.X = int.Parse(temp[2]);
                        tempMove._prevPoint.Y = int.Parse(temp[3]);

                        if(tempMove._pressPoint != null ) _pressBtn.Location = tempMove._pressPoint;
                        if(tempMove._prevPoint != null) _prevBtn.Location = tempMove._prevPoint;
                    }));
                    form._tcpClient.Client.BeginReceive(form._buffer, 0, form._buffer.Length, SocketFlags.None, CallReceive, form);

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "ERROR (CallReceive)");
                    AddToJournal($"ERROR (CallReceive) - {ex.Message}\r\n");
                }
            }
        }

        public struct MoveInfo
        {
            public Point _pressPoint;
            public Point _prevPoint;
        }


        // Смена текущего игрока
        public void SwitchPlayer()
        {
            _currentPlayer = _currentPlayer == 1 ? 2 : 1;
        }

        public void FirstPlayer(bool isWhite)
        {
            if (isWhite) _currentPlayer = 1;
            else _currentPlayer = 2;
        }

        // Определение цвета фона кнопки
        public Color GetPrevButtonColor(Button prevButton)
        {
            if ((prevButton.Location.Y / _cellSize % 2) != 0)
            {
                if ((prevButton.Location.X / _cellSize % 2) == 0)
                {
                    return Color.DarkBlue;
                }
            }
            if ((prevButton.Location.Y / _cellSize) % 2 == 0)
            {
                if ((prevButton.Location.X / _cellSize) % 2 != 0)
                {
                    return Color.DarkBlue;
                }
            }
            return Color.White;
        }

        // Обработчик события нажатия на кнопку с шашкой
        public void OnFigurePress(object sender, EventArgs e)
        {
            if (InvokeRequired)
            {
                Invoke(new MethodInvoker(() => OnFigurePress(sender, e)));
                return;
            }

            // Если была выбрана предыдущая кнопка и текущий игрок может ходить
            if ((_prevBtn != null) && ((_isServer && _currentPlayer == 1) || (!_isServer && _currentPlayer == 2)))
                _prevBtn.BackColor = GetPrevButtonColor(_prevBtn);

            _pressBtn = sender as Button;
            Point _pressPoint = new Point();

            _pressPoint.X = _pressBtn.Location.X / _cellSize;
            _pressPoint.Y = _pressBtn.Location.Y / _cellSize;

            // Если на текущей кнопке стоит фигура текущего игрока
            if ((_map[_pressPoint.Y, _pressPoint.X] != 0) && (_map[_pressPoint.Y, _pressPoint.X] == _currentPlayer))
            {
                _pressBtn.BackColor = Color.Red;
                _isMoving = true; // Устанавливаем флаг, что игрок начал ход
            }
            else
            {
                if (_isMoving)
                {
                    Point _prevPoint = new Point();

                    _prevPoint.X = _pressBtn.Location.X / _cellSize;
                    _prevPoint.Y = _pressBtn.Location.Y / _cellSize;

                    int temp = _map[_pressPoint.Y, _pressPoint.X];
                    _map[_pressPoint.Y, _pressPoint.X] = _map[_prevPoint.Y, _prevPoint.X];
                    _map[_prevPoint.Y, _prevPoint.X] = temp;
                    _pressBtn.Image = _prevBtn.Image;
                    _prevBtn.Image = null;

                    _moveInfo._pressPoint = _pressPoint;
                    _moveInfo._prevPoint = _prevPoint;
                    ThreadPool.QueueUserWorkItem(ReceiveOrSend, this);
                    _isMoving = false; // Сбрасываем флаг, что игрок завершил ход
                    SwitchPlayer(); // Меняем текущего игрока
                    _pressBtn.Invalidate();
                    _prevBtn.Invalidate();
                    Update();
                }
            }

            _prevBtn = _pressBtn;
        }
   
        // Обработчик события выбора режима "Сервер"
        private void rbIsServer_Checked(object sender, EventArgs e)
        {
            if (rbIsServer.Checked)
            {
                _isServer = true;
            }
        }

        // Обработчик события выбора режима "Клиент"
        private void rbIsClient_Checked(object sender, EventArgs e)
        {
            if (rbIsClient.Checked)
            {
                _isServer = false;
            }
        }

        // Делегат для обновления журнала событий
        public delegate void AddToJournalDel(string msg);

        // Метод для добавления записи в журнал
        private void AddToJournal(string msg)
        {
            if (lbJournal.InvokeRequired)
            {
                lbJournal.Invoke(new AddToJournalDel(AddToJournal), msg);
            }
            else
            {
                lbJournal.Items.Add(msg);
            }
        }

        // Закрытие сокетов при закрытии приложения
        private void ClosingTcp()
        {
            // Закрытие TcpListener и TcpClient
            if (_tcpServer != null)
            {
                _tcpServer.Stop();
            }
            if (_tcpClient != null && _tcpClient.Connected)
            {
                _tcpClient.Close();
            }
        }

        // Обработчик события нажатия кнопки "Закрыть"
        private void btnClose_Click(object sender, EventArgs e)
        {
            ClosingTcp(); // Закрываем сокеты
            Close(); // Закрываем приложение
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO.Ports ;
using System.Threading;
using System.Runtime.InteropServices;
using System.IO;

namespace 端口侦听控制系统
{
    public partial class frm_main : Form
    {

        #region 参数


        #region 侦听相关

        //侦听中标志
        bool flag_in_listening = false ;

        //侦听端口
        SerialPort sp_listening;

        //先前侦听结果
        string listening_old_state = "";

        //端口监听线程
        Thread thread_sp_listening;

        //侦听线程轮询时间
        int listening_thread_sleep = 8;

        #endregion


        #region 触发相关

        //原先的触发状态
        string trigger_old_state;

        //触发端口1标志
        bool flag_output_1_trigger = false;
        //触发端口2标志
        bool flag_output_2_trigger = false;
        //触发端口3标志
        bool flag_output_3_trigger = false;
        //触发端口4标志
        bool flag_output_4_trigger = false;

        #endregion


        #region 输出相关

        //输出位置1线程
        Thread thread_output_position_1_proess;
        //输出位置2线程
        Thread thread_output_position_2_proess;
        //输出位置3线程
        Thread thread_output_position_3_proess;
        //输出位置4线程
        Thread thread_output_position_4_proess;

        //输出位置1串口
        SerialPort sp_output_position_1;
        //输出位置2串口
        SerialPort sp_output_position_2;
        //输出位置3串口
        SerialPort sp_output_position_3;
        //输出位置4串口
        SerialPort sp_output_position_4;


        //触发位置1定时器
        System.Timers .Timer timer_output_1;
        //触发位置2定时器
        System.Timers.Timer timer_output_2;
        //触发位置3定时器
        System.Timers.Timer timer_output_3;
        //触发位置4定时器
        System.Timers.Timer timer_output_4;

        //输出位置1输出中标志
        bool flag_output_position_1_in_output;
        //输出位置2输出中标志
        bool flag_output_position_2_in_output;
        //输出位置3输出中标志
        bool flag_output_position_3_in_output;
        //输出位置4输出中标志
        bool flag_output_position_4_in_output;

        //输出位置1次数计数器(用于计算总的输出次数)
        int output_position_1_output_indicator ;
        //输出位置2次数计数器(用于计算总的输出次数)
        int output_position_2_output_indicator;
        //输出位置3次数计数器(用于计算总的输出次数)
        int output_position_3_output_indicator;
        //输出位置4次数计数器(用于计算总的输出次数)
        int output_position_4_output_indicator;

        //输出位置1循环内触发计数器(用于计算循环内触发计数)
        int output_position_1_trigger_indicator;
        //输出位置2循环内触发计数器(用于计算循环内触发计数)
        int output_position_2_trigger_indicator;
        //输出位置3循环内触发计数器(用于计算循环内触发计数)
        int output_position_3_trigger_indicator;
        //输出位置4循环内触发计数器(用于计算循环内触发计数)
        int output_position_4_trigger_indicator;

        //输出位置1循环计数器(用于计算循环次数的统计)
        int output_position_1_recycle_indicator;
        //输出位置2循环计数器(用于计算循环次数的统计)
        int output_position_2_recycle_indicator;
        //输出位置3循环计数器(用于计算循环次数的统计)
        int output_position_3_recycle_indicator;
        //输出位置4循环计数器(用于计算循环次数的统计)
        int output_position_4_recycle_indicator;


        //输出线程轮询时间
        int output_thread_sleep = 2;

        //发送消息时的窗体名称
        string message_function_window_name;

        #endregion


        #region 模拟触发


        //触发位置1定时器
        System.Timers.Timer timer_simulate;
        //模拟触发中标志
        bool flag_in_simulate = false;

        #endregion


        #region 额外追加输出

        //端口1额外输出定时器
        System.Timers.Timer timer_output_addition_1;
        //端口1额外输出定时器计数器
        int addtional_timer_1_amount;

        //端口2额外输出定时器
        System.Timers.Timer timer_output_addition_2;
        //端口2额外输出定时器计数器
        int addtional_timer_2_amount;

        //端口3额外输出定时器
        System.Timers.Timer timer_output_addition_3;
        //端口3额外输出定时器计数器
        int addtional_timer_3_amount;

        //端口4额外输出定时器
        System.Timers.Timer timer_output_addition_4;
        //端口4额外输出定时器计数器
        int addtional_timer_4_amount;


        #endregion


        #region 日志相关

        //侦听日志互斥量
        public object mutex_log_listen = new object ();
        //输出日志互斥量
        public object mutex_log_output = new object ();

        #endregion


        #endregion


        #region Windows API


        #region WINAPI FindWindow
        [DllImport("user32.dll", EntryPoint = "FindWindow", SetLastError = true)]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
        #endregion


        #region WINAPI FindWindowEx
        [DllImport("user32.dll", EntryPoint = "FindWindowEx", SetLastError = true)]
        private static extern IntPtr FindWindowEx(IntPtr hwndParent, uint hwndChildAfter, string lpszClass, string lpszWindow);
        #endregion


        #region WINAPI SendMessage
        [DllImport("user32.dll", EntryPoint = "SendMessage", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern int SendMessage(IntPtr hwnd, uint wMsg, int wParam, int lParam);
        #endregion


        #region WINAPI SetForegroundWindow
        [DllImport("user32.dll", EntryPoint = "SetForegroundWindow", SetLastError = true)]
        private static extern void SetForegroundWindow(IntPtr hwnd);
        #endregion


        #region WINAPI EnumChildWindows
        [DllImport("user32.dll", EntryPoint = "EnumChildWindows", SetLastError = true)]
        private static extern bool EnumChildWindows(IntPtr hWndParent, EnumWindowProc callback, IntPtr lParam);
        private delegate bool EnumWindowProc(IntPtr hWnd, IntPtr lParam);
        #endregion


        #region WINAPI GetClassName
        [DllImport("user32.dll", EntryPoint = "GetClassName", SetLastError = true)]
        private static extern IntPtr GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxcount);
        #endregion


        #region WINAPI GetWindowText
        [DllImport("user32.dll", EntryPoint = "GetWindowText", SetLastError = true)]
        private static extern int GetWindowText(IntPtr  hWnd, StringBuilder lpString, int nMaxCount);

        #endregion


        #endregion


        public frm_main()
        {
            InitializeComponent();

            //界面居中显示
            this.StartPosition = FormStartPosition.CenterScreen ;

            //允许控件间线程调用
            CheckForIllegalCrossThreadCalls = false;

            //初始化窗体
            frm_main_init();
        }

      

        #region 按键功能


        #region 侦听


        #region 启动侦听/终止侦听
        /// <summary>
        /// 函数功能：启动侦听
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_listening_start_Click(object sender, EventArgs e)
        {
           // try
          //  {
                if (btn_listening_start.Text.Trim().ToString() == "启动侦听")
                {
                    //端口设置
                    sp_listening = new SerialPort();
                    sp_listening.DataReceived += port_DataReceived;
                    //端口名
                    sp_listening.PortName = 端口侦听控制系统.Properties.Settings.Default.grp_listening_comunication_config_port_name;
                    //波特率
                    sp_listening.BaudRate = 端口侦听控制系统.Properties.Settings.Default.grp_listening_comunication_config_baud_rate;
                    //数据位
                    sp_listening.DataBits = 端口侦听控制系统.Properties.Settings.Default.grp_listening_comunication_config_databit;
                    // 奇偶校验
                    switch (端口侦听控制系统.Properties.Settings.Default.grp_listening_comunication_config_parity)
                    {
                        case "None":
                            sp_listening.Parity = Parity.None;
                            break;
                        case "Odd":
                            sp_listening.Parity = Parity.Odd;
                            break;
                        case "Even":
                            sp_listening.Parity = Parity.Even;
                            break;
                        case "Mark:":
                            sp_listening.Parity = Parity.Mark;
                            break;
                        case "Space":
                            sp_listening.Parity = Parity.Space;
                            break;
                    }

                    // 停止位
                    switch (端口侦听控制系统.Properties.Settings.Default.grp_listening_comunication_config_stopbit)
                    {
                        case "One":
                            sp_listening.StopBits = StopBits.One;
                            break;
                        case "Two":
                            sp_listening.StopBits = StopBits.Two;
                            break;
                        case "OnePointFive":
                            sp_listening.StopBits = StopBits.OnePointFive;
                            break;
                    }

                   //按键变为终止侦听
                    btn_listening_start.Text = "终止侦听";
                    //侦听通讯设置不可用
                    btn_listening_communication_config.Enabled = false;
                    //开机自动启动不可用
                    checkbox_autorun_listening.Enabled = false;
                    //开始模拟变为不可用
                    btn_start_simulate.Enabled = false;
                    //重置先前侦听状态
                    listening_old_state = "1111111";
                    //重置先前的触发状态
                    trigger_old_state = "11111111";
                    //设置侦听中标志
                    flag_in_listening = true;
                    //初始化listview_log_listen 
                    listview_log_listen.Items.Clear();

                    lbl_trigger_position_1_indicator.Text = "0";
                    //重置输出位置1次数计数器(用于计算总的输出次数)
                    output_position_1_output_indicator = 0;
                    lbl_output_position_1_indicator.Text = "0";
                    //输出位置1循环计数器(用于计算循环次数的统计)
                    output_position_1_recycle_indicator = 0;
                    lbl_output_position_1_recycle_indicator.Text = "0";

                    lbl_trigger_position_2_indicator.Text = "0";
                    //重置输出位置2次数计数器(用于计算总的输出次数)
                    output_position_2_output_indicator = 0;
                    lbl_output_position_2_indicator.Text = "0";
                    //输出位置2循环计数器(用于计算循环次数的统计)
                    output_position_2_recycle_indicator = 0;
                    lbl_output_position_2_recycle_indicator.Text = "0";

                    lbl_trigger_position_3_indicator.Text = "0";
                    //重置输出位置3次数计数器(用于计算总的输出次数)
                    output_position_3_output_indicator = 0;
                    lbl_output_position_3_indicator.Text = "0";
                    //输出位置3循环计数器(用于计算循环次数的统计)
                    output_position_3_recycle_indicator = 0;
                    lbl_output_position_3_recycle_indicator.Text = "0";

                    lbl_trigger_position_4_indicator.Text = "0";
                    //重置输出位置4次数计数器(用于计算总的输出次数)
                    output_position_4_output_indicator = 0;
                    lbl_output_position_4_indicator.Text = "0";
                    //输出位置4循环计数器(用于计算循环次数的统计)
                    output_position_4_recycle_indicator = 0;
                    lbl_output_position_4_recycle_indicator.Text = "0";
                    //设置标签文字及颜色
                    lbl_listening_port_name_tag.Text = "正在侦听端口：";
                    lbl_listening_port_name_tag.ForeColor = Color.Blue;
                    lbl_listening_port_name.ForeColor = Color.Blue;
                    //打开端口
                    sp_listening.Open();
                    //sp_listening.DataReceived += port_DataReceived;
                    //启动侦听线程
                    // MethodInvoker my_methodinvoker = new MethodInvoker(thread_begin_serial_port_listening);
                    // my_methodinvoker.BeginInvoke(null, null);
                }
                else if (btn_listening_start.Text.Trim().ToString() == "终止侦听")
                {

                    //按键变为启动侦听
                    btn_listening_start.Text = "启动侦听";
                    //侦听通讯设置可用
                    btn_listening_communication_config.Enabled = true;
                    //开机自动启动可用
                    checkbox_autorun_listening.Enabled = true;
                    //开始模拟变为可用
                    btn_start_simulate.Enabled = true;
                    //取消侦听中标志
                    flag_in_listening = false;
                    //设置标签文字及颜色
                    lbl_listening_port_name_tag.Text = "端口：";
                    lbl_listening_port_name_tag.ForeColor = Color.Black;
                    lbl_listening_port_name.ForeColor = Color.Black;
                    sp_listening.Close();
                    sp_listening.Dispose();
                }

                

       //     }
       //     catch (Exception)
       //     {
                //MessageBox.Show("错误：" + ee.ToString(), "提示", MessageBoxButtons.OK);
                //错误显示
      //          error_message_show();
                //退出程序
      //          Environment.Exit(0);
      //      }
      //      finally
      //      {
      //      }

        }
        #endregion

        void port_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            //  byte[] buffer = new byte[port.BytesToRead];
            //  port.Read(buffer, 0, buffer.Length);
            //在这里对接收到的数据进行处理

            //  AddData(buffer);

            byte[] buffer = new byte[13];
            sp_listening.Read(buffer, 0, buffer.Length);
            //在这里对接收到的数据进行处理
            //Getms(Convert.ToString(buffer[0],16));
            AddData(buffer);

            // 如果有返回数据
            if (buffer.Length > 0)
            {
                //且返回数据与上一状态不同
               // string t = Convert.ToString(buffer[9], 2);
                if (Convert.ToString(buffer[9], 2) != listening_old_state)
                {

                    // 最后状态赋值
                    listening_old_state = Convert.ToString(buffer[9], 2);
                   // MessageBox.Show(listening_old_state);

                    //判断触发位置
                    trigger_positon_estimate(listening_old_state.Substring(4, 4));

                    //触发状态赋值
                    trigger_old_state = listening_old_state.Substring(4, 4);

                    //添加到listview_log
                    listview_log_listen_add_data(trigger_old_state);

                }
            }

        }

        public void AddData(byte[] data)
        {

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < data.Length; i++)
            {
                sb.AppendFormat("{0:x2}" + " ", data[i]);
            }
            //sb.Append("\r\n");
            listview_log_listen_add_data(sb.ToString().ToUpper());
           // AddContent(sb.ToString().ToUpper());


        }

        #region 通讯设置
        /// <summary>
        /// 函数功能：通讯设置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_listening_communication_config_Click(object sender, EventArgs e)
        {
            try
            {
                frm_communication_config my_frm_communication_config = new frm_communication_config(this);
                my_frm_communication_config.Tag = "listening";

                //设置窗体赋值

                //端口名
                my_frm_communication_config.txtbox_communication_config_port.Text = 端口侦听控制系统.Properties.Settings.Default.grp_listening_comunication_config_port_name.ToString ();
                //波特率
                my_frm_communication_config.txtbox_communication_config_baud_rate.Text = 端口侦听控制系统.Properties.Settings.Default.grp_listening_comunication_config_baud_rate.ToString();
                //数据位
                my_frm_communication_config.txtbox_communication_config_databit.Text = 端口侦听控制系统.Properties.Settings.Default.grp_listening_comunication_config_databit.ToString();
                //校验位
                my_frm_communication_config.cmb_communication_config_parity.Text = 端口侦听控制系统.Properties.Settings.Default.grp_listening_comunication_config_parity.ToString();
                //停止位
                my_frm_communication_config.cmb_communication_config_stopbit.Text = 端口侦听控制系统.Properties.Settings.Default.grp_listening_comunication_config_stopbit.ToString();

                //弹出设置窗体        
                my_frm_communication_config.ShowDialog();
            }
            catch (Exception ee)
            {
                MessageBox.Show("错误：" + ee.ToString(), "提示", MessageBoxButtons.OK);

                //退出程序
                Environment.Exit(0);
            }
            finally
            {
            }
        }
        #endregion

     
        #endregion


        #region 测试输出


        #region 触发位置一测试输出
        /// <summary>
        /// 函数功能：触发位置一测试输出
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_output_1_test_Click(object sender, EventArgs e)
        {
            try
            {

                //判断输出方式
                //如果是端口输出
                if (端口侦听控制系统.Properties.Settings.Default.grp_output_1_type.ToString() == "0")
                {
                    //端口设置
                    sp_output_position_1 = new SerialPort();

                    //端口名
                    sp_output_position_1.PortName = 端口侦听控制系统.Properties.Settings.Default.grp_output_1_port_comunication_config_port_name;
                    //波特率
                    sp_output_position_1.BaudRate = 端口侦听控制系统.Properties.Settings.Default.grp_output_1_port_comunication_config_baud_rate;
                    //数据位
                    sp_output_position_1.DataBits = 端口侦听控制系统.Properties.Settings.Default.grp_output_1_port_comunication_config_databit;
                    // 奇偶校验
                    switch (端口侦听控制系统.Properties.Settings.Default.grp_output_1_port_comunication_config_parity)
                    {
                        case "None":
                            sp_output_position_1.Parity = Parity.None;
                            break;
                        case "Odd":
                            sp_output_position_1.Parity = Parity.Odd;
                            break;
                        case "Even":
                            sp_output_position_1.Parity = Parity.Even;
                            break;
                        case "Mark:":
                            sp_output_position_1.Parity = Parity.Mark;
                            break;
                        case "Space":
                            sp_output_position_1.Parity = Parity.Space;
                            break;
                    }

                    // 停止位
                    switch (端口侦听控制系统.Properties.Settings.Default.grp_output_1_port_comunication_config_stopbit)
                    {
                        case "One":
                            sp_output_position_1.StopBits = StopBits.One;
                            break;
                        case "Two":
                            sp_output_position_1.StopBits = StopBits.Two;
                            break;
                        case "OnePointFive":
                            sp_output_position_1.StopBits = StopBits.OnePointFive;
                            break;
                    }



                    //如果要写入的数据不为空
                    if (端口侦听控制系统.Properties.Settings.Default.grp_output_1_port_write_data.Trim().Length > 0)
                    {

                        //如果输出端口未打开，打开输出端口
                        if (!sp_output_position_1.IsOpen)
                        {
                            sp_output_position_1.Open();
                        }

                        //写入数据                            
                        //取得写入的数据
                        string[] sp_data = 端口侦听控制系统.Properties.Settings.Default.grp_output_1_port_write_data.Split(' ');
                        byte[] byte_data = new byte[sp_data.Length];
                        int out_result = 0;
                        for (int i = 0; i < sp_data.Length; i++)
                        {
                            int.TryParse(sp_data[i], out out_result);

                            byte_data[i] = (byte)out_result;
                        }
                        sp_output_position_1.Write(byte_data, 0, byte_data.Length);
                    }

                    //关闭端口
                    sp_output_position_1.Close();
                    //释放端口
                    sp_output_position_1.Dispose();

                }
                //如果是sendmessage 输出
                else if (端口侦听控制系统.Properties.Settings.Default.grp_output_1_type.ToString() == "1")
                {
                    //发送消息
                    winapi_get_and_click(端口侦听控制系统.Properties.Settings.Default.grp_output_1_message_data_1, 端口侦听控制系统.Properties.Settings.Default.grp_output_1_message_data_2);
                }
                //如果是消息输出
                else if (端口侦听控制系统.Properties.Settings.Default.grp_output_1_type.ToString() == "2")
                {
                    //发送消息
                    winapi_send_window_message(端口侦听控制系统.Properties.Settings.Default.grp_output_1_window_data_1, 端口侦听控制系统.Properties.Settings.Default.grp_output_1_window_data_2);
                }
               
            }
            catch (Exception)
            {
                //MessageBox.Show("错误：" + ee.ToString(), "提示", MessageBoxButtons.OK);
                //错误显示
                error_message_show();
                //退出程序
                Environment.Exit(0);
            }
            finally
            {
            }
        }
        #endregion


        #region 触发位置二测试输出
        /// <summary>
        /// 函数功能：触发位置二测试输出
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_output_2_test_Click(object sender, EventArgs e)
        {
            try
            {
                //判断输出方式
                //如果是端口输出
                if (端口侦听控制系统.Properties.Settings.Default.grp_output_2_type.ToString() == "0")
                {
                    //端口设置
                    sp_output_position_2 = new SerialPort();

                    //端口名
                    sp_output_position_2.PortName = 端口侦听控制系统.Properties.Settings.Default.grp_output_2_port_comunication_config_port_name;
                    //波特率
                    sp_output_position_2.BaudRate = 端口侦听控制系统.Properties.Settings.Default.grp_output_2_port_comunication_config_baud_rate;
                    //数据位
                    sp_output_position_2.DataBits = 端口侦听控制系统.Properties.Settings.Default.grp_output_2_port_comunication_config_databit;
                    // 奇偶校验
                    switch (端口侦听控制系统.Properties.Settings.Default.grp_output_2_port_comunication_config_parity)
                    {
                        case "None":
                            sp_output_position_2.Parity = Parity.None;
                            break;
                        case "Odd":
                            sp_output_position_2.Parity = Parity.Odd;
                            break;
                        case "Even":
                            sp_output_position_2.Parity = Parity.Even;
                            break;
                        case "Mark:":
                            sp_output_position_2.Parity = Parity.Mark;
                            break;
                        case "Space":
                            sp_output_position_2.Parity = Parity.Space;
                            break;
                    }

                    // 停止位
                    switch (端口侦听控制系统.Properties.Settings.Default.grp_output_2_port_comunication_config_stopbit)
                    {
                        case "One":
                            sp_output_position_2.StopBits = StopBits.One;
                            break;
                        case "Two":
                            sp_output_position_2.StopBits = StopBits.Two;
                            break;
                        case "OnePointFive":
                            sp_output_position_2.StopBits = StopBits.OnePointFive;
                            break;
                    }



                    //如果要写入的数据不为空
                    if (端口侦听控制系统.Properties.Settings.Default.grp_output_2_port_write_data.Trim().Length > 0)
                    {

                        //如果输出端口未打开，打开输出端口
                        if (!sp_output_position_2.IsOpen)
                        {
                            sp_output_position_2.Open();
                        }

                        //写入数据                            
                        //取得写入的数据
                        string[] sp_data = 端口侦听控制系统.Properties.Settings.Default.grp_output_2_port_write_data.Split(' ');
                        byte[] byte_data = new byte[sp_data.Length];
                        int out_result = 0;
                        for (int i = 0; i < sp_data.Length; i++)
                        {
                            int.TryParse(sp_data[i], out out_result);

                            byte_data[i] = (byte)out_result;
                        }
                        sp_output_position_2.Write(byte_data, 0, byte_data.Length);
                    }

                    //关闭端口
                    sp_output_position_2.Close();
                    //释放端口
                    sp_output_position_2.Dispose();

                }
                //如果是sendmessage 输出
                else if (端口侦听控制系统.Properties.Settings.Default.grp_output_2_type.ToString() == "1")
                {
                    //发送消息
                    winapi_get_and_click(端口侦听控制系统.Properties.Settings.Default.grp_output_2_message_data_1, 端口侦听控制系统.Properties.Settings.Default.grp_output_2_message_data_2);
                }
                //如果是消息输出
                else if (端口侦听控制系统.Properties.Settings.Default.grp_output_2_type.ToString() == "2")
                {
                    //发送消息
                    winapi_send_window_message(端口侦听控制系统.Properties.Settings.Default.grp_output_2_window_data_1, 端口侦听控制系统.Properties.Settings.Default.grp_output_2_window_data_2);
                }
            }
            catch (Exception)
            {
                // MessageBox.Show("错误：" + ee.ToString(), "提示", MessageBoxButtons.OK);
                //错误显示
                error_message_show();
                //退出程序
                Environment.Exit(0);
            }
            finally
            {
            }
        }

        #endregion


        #region 触发位置三测试输出
        /// <summary>
        /// 函数功能：触发位三测试输出
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_output_3_test_Click(object sender, EventArgs e)
        {
            try
            {
                //判断输出方式
                //如果是端口输出
                if (端口侦听控制系统.Properties.Settings.Default.grp_output_3_type.ToString() == "0")
                {
                    //端口设置
                    sp_output_position_3 = new SerialPort();

                    //端口名
                    sp_output_position_3.PortName = 端口侦听控制系统.Properties.Settings.Default.grp_output_3_port_comunication_config_port_name;
                    //波特率
                    sp_output_position_3.BaudRate = 端口侦听控制系统.Properties.Settings.Default.grp_output_3_port_comunication_config_baud_rate;
                    //数据位
                    sp_output_position_3.DataBits = 端口侦听控制系统.Properties.Settings.Default.grp_output_3_port_comunication_config_databit;
                    // 奇偶校验
                    switch (端口侦听控制系统.Properties.Settings.Default.grp_output_3_port_comunication_config_parity)
                    {
                        case "None":
                            sp_output_position_3.Parity = Parity.None;
                            break;
                        case "Odd":
                            sp_output_position_3.Parity = Parity.Odd;
                            break;
                        case "Even":
                            sp_output_position_3.Parity = Parity.Even;
                            break;
                        case "Mark:":
                            sp_output_position_3.Parity = Parity.Mark;
                            break;
                        case "Space":
                            sp_output_position_3.Parity = Parity.Space;
                            break;
                    }

                    // 停止位
                    switch (端口侦听控制系统.Properties.Settings.Default.grp_output_3_port_comunication_config_stopbit)
                    {
                        case "One":
                            sp_output_position_3.StopBits = StopBits.One;
                            break;
                        case "Two":
                            sp_output_position_3.StopBits = StopBits.Two;
                            break;
                        case "OnePointFive":
                            sp_output_position_3.StopBits = StopBits.OnePointFive;
                            break;
                    }



                    //如果要写入的数据不为空
                    if (端口侦听控制系统.Properties.Settings.Default.grp_output_3_port_write_data.Trim().Length > 0)
                    {

                        //如果输出端口未打开，打开输出端口
                        if (!sp_output_position_3.IsOpen)
                        {
                            sp_output_position_3.Open();
                        }

                        //写入数据                            
                        //取得写入的数据
                        string[] sp_data = 端口侦听控制系统.Properties.Settings.Default.grp_output_3_port_write_data.Split(' ');
                        byte[] byte_data = new byte[sp_data.Length];
                        int out_result = 0;
                        for (int i = 0; i < sp_data.Length; i++)
                        {
                            int.TryParse(sp_data[i], out out_result);

                            byte_data[i] = (byte)out_result;
                        }
                        sp_output_position_3.Write(byte_data, 0, byte_data.Length);
                    }

                    //关闭端口
                    sp_output_position_3.Close();
                    //释放端口
                    sp_output_position_3.Dispose();

                }
                //如果是sendmessage 输出
                else if (端口侦听控制系统.Properties.Settings.Default.grp_output_3_type.ToString() == "1")
                {
                    //发送消息
                    winapi_get_and_click(端口侦听控制系统.Properties.Settings.Default.grp_output_3_message_data_1, 端口侦听控制系统.Properties.Settings.Default.grp_output_3_message_data_2);
                }
                //如果是消息输出
                else if (端口侦听控制系统.Properties.Settings.Default.grp_output_3_type.ToString() == "2")
                {
                    //发送消息
                    winapi_send_window_message(端口侦听控制系统.Properties.Settings.Default.grp_output_3_window_data_1, 端口侦听控制系统.Properties.Settings.Default.grp_output_3_window_data_2);
                }

            }
            catch (Exception)
            {
                //MessageBox.Show("错误：" + ee.ToString(), "提示", MessageBoxButtons.OK);
                //错误显示
                error_message_show();
                //退出程序
                Environment.Exit(0);
            }
            finally
            {
            }
        }
        #endregion


        #region 触发位置四测试输出
        /// <summary>
        /// 函数功能：触发位置四测试输出
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_output_4_test_Click(object sender, EventArgs e)
        {
            try
            {
                //判断输出方式
                //如果是端口输出
                if (端口侦听控制系统.Properties.Settings.Default.grp_output_4_type.ToString() == "0")
                {
                    //端口设置
                    sp_output_position_4 = new SerialPort();

                    //端口名
                    sp_output_position_4.PortName = 端口侦听控制系统.Properties.Settings.Default.grp_output_4_port_comunication_config_port_name;
                    //波特率
                    sp_output_position_4.BaudRate = 端口侦听控制系统.Properties.Settings.Default.grp_output_4_port_comunication_config_baud_rate;
                    //数据位
                    sp_output_position_4.DataBits = 端口侦听控制系统.Properties.Settings.Default.grp_output_4_port_comunication_config_databit;
                    // 奇偶校验
                    switch (端口侦听控制系统.Properties.Settings.Default.grp_output_4_port_comunication_config_parity)
                    {
                        case "None":
                            sp_output_position_4.Parity = Parity.None;
                            break;
                        case "Odd":
                            sp_output_position_4.Parity = Parity.Odd;
                            break;
                        case "Even":
                            sp_output_position_4.Parity = Parity.Even;
                            break;
                        case "Mark:":
                            sp_output_position_4.Parity = Parity.Mark;
                            break;
                        case "Space":
                            sp_output_position_4.Parity = Parity.Space;
                            break;
                    }

                    // 停止位
                    switch (端口侦听控制系统.Properties.Settings.Default.grp_output_4_port_comunication_config_stopbit)
                    {
                        case "One":
                            sp_output_position_4.StopBits = StopBits.One;
                            break;
                        case "Two":
                            sp_output_position_4.StopBits = StopBits.Two;
                            break;
                        case "OnePointFive":
                            sp_output_position_4.StopBits = StopBits.OnePointFive;
                            break;
                    }



                    //如果要写入的数据不为空
                    if (端口侦听控制系统.Properties.Settings.Default.grp_output_4_port_write_data.Trim().Length > 0)
                    {

                        //如果输出端口未打开，打开输出端口
                        if (!sp_output_position_4.IsOpen)
                        {
                            sp_output_position_4.Open();
                        }

                        //写入数据                            
                        //取得写入的数据
                        string[] sp_data = 端口侦听控制系统.Properties.Settings.Default.grp_output_4_port_write_data.Split(' ');
                        byte[] byte_data = new byte[sp_data.Length];
                        int out_result = 0;
                        for (int i = 0; i < sp_data.Length; i++)
                        {
                            int.TryParse(sp_data[i], out out_result);

                            byte_data[i] = (byte)out_result;
                        }
                        sp_output_position_4.Write(byte_data, 0, byte_data.Length);
                    }

                    //关闭端口
                    sp_output_position_4.Close();
                    //释放端口
                    sp_output_position_4.Dispose();

                }
                //如果是sendmessage 输出
                else if (端口侦听控制系统.Properties.Settings.Default.grp_output_4_type.ToString() == "1")
                {
                    //发送消息
                    winapi_get_and_click(端口侦听控制系统.Properties.Settings.Default.grp_output_4_message_data_1, 端口侦听控制系统.Properties.Settings.Default.grp_output_4_message_data_2);
                }
                //如果是消息输出
                else if (端口侦听控制系统.Properties.Settings.Default.grp_output_4_type.ToString() == "2")
                {
                    //发送消息
                    winapi_send_window_message(端口侦听控制系统.Properties.Settings.Default.grp_output_4_window_data_1, 端口侦听控制系统.Properties.Settings.Default.grp_output_4_window_data_2);
                }

            }
            catch (Exception)
            {
                //MessageBox.Show("错误：" + ee.ToString(), "提示", MessageBoxButtons.OK);
                //错误显示
                error_message_show();
                //退出程序
                Environment.Exit(0);
            }
            finally
            {
            }
        }
        #endregion


        #endregion


        #region 输出



        #region 触发位置一


        #region 启动输出/终止输出
        /// <summary>
        /// 函数功能：启动输出
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_output_1_start_Click(object sender, EventArgs e)
        {
            try
            {
                if (btn_output_1_start.Text.Trim().ToString() == "启动输出")
                {

                    //端口设置
                    sp_output_position_1 = new SerialPort();

                    //端口名
                    sp_output_position_1.PortName = 端口侦听控制系统.Properties.Settings.Default.grp_output_1_port_comunication_config_port_name;
                    //波特率
                    sp_output_position_1.BaudRate = 端口侦听控制系统.Properties.Settings.Default.grp_output_1_port_comunication_config_baud_rate;
                    //数据位
                    sp_output_position_1.DataBits = 端口侦听控制系统.Properties.Settings.Default.grp_output_1_port_comunication_config_databit;
                    // 奇偶校验
                    switch (端口侦听控制系统.Properties.Settings.Default.grp_output_1_port_comunication_config_parity)
                    {
                        case "None":
                            sp_output_position_1.Parity = Parity.None;
                            break;
                        case "Odd":
                            sp_output_position_1.Parity = Parity.Odd;
                            break;
                        case "Even":
                            sp_output_position_1.Parity = Parity.Even;
                            break;
                        case "Mark:":
                            sp_output_position_1.Parity = Parity.Mark;
                            break;
                        case "Space":
                            sp_output_position_1.Parity = Parity.Space;
                            break;
                    }

                    // 停止位
                    switch (端口侦听控制系统.Properties.Settings.Default.grp_output_1_port_comunication_config_stopbit)
                    {
                        case "One":
                            sp_output_position_1.StopBits = StopBits.One;
                            break;
                        case "Two":
                            sp_output_position_1.StopBits = StopBits.Two;
                            break;
                        case "OnePointFive":
                            sp_output_position_1.StopBits = StopBits.OnePointFive;
                            break;
                    }


                    //按键变为"终止输出"
                    btn_output_1_start.Text = "终止输出";

                    //控制设置不可用
                    btn_output_1_control_config.Enabled = false;
                    //开机自动启动
                    checkbox_autorun_output_1.Enabled = false;
                    //测试输出不可用
                    btn_output_1_test.Enabled = false;
                    //锁定输出组
                    tab_control_output_1.Enabled = false;
                    lbl_trigger_position_1_indicator.Text = "0";
                    lbl_trigger_position_1_indicator.ForeColor = Color.Blue;
                    //重置输出位置1次数计数器(用于计算总的输出次数)
                    output_position_1_output_indicator = 0;
                    lbl_output_position_1_indicator.Text = "0";
                    lbl_output_position_1_indicator.ForeColor = Color.Blue;
                    //重置输出位置1循环内触发计数器(用于计算循环内触发计数)
                    output_position_1_trigger_indicator = 0;
                    //输出位置1循环计数器(用于计算循环次数的统计)
                    output_position_1_recycle_indicator = 0;
                    lbl_output_position_1_recycle_indicator.Text = "0";
                    lbl_output_position_1_recycle_indicator.ForeColor = Color.Blue;
                    //设置位置标签颜色
                    lbl_trigger_position_1_tag.ForeColor = Color.Blue;
                    //重置触发端口1标志
                    flag_output_1_trigger = false;
                    //输出中标志
                    flag_output_position_1_in_output = true;
                    //初始化定时器与设置时间间隔
                    timer_output_1 = new System.Timers.Timer(端口侦听控制系统.Properties.Settings.Default.grp_output_1_control_config_series_shoot_timer);
                    //设置定时器事件
                    timer_output_1.Elapsed += new System.Timers.ElapsedEventHandler(timer_output_1_tick);
                    if (端口侦听控制系统.Properties.Settings.Default.grp_output_1_control_config_additional_interval > 0)
                    {
                        //初始化额外输出定时器
                        timer_output_addition_1 = new System.Timers.Timer(端口侦听控制系统.Properties.Settings.Default.grp_output_1_control_config_additional_interval);
                    }
                    else
                    {
                        //初始化额外输出定时器
                        timer_output_addition_1 = new System.Timers.Timer(10);
                    }
                    //额外输出定时器事件
                    timer_output_addition_1.Elapsed += new System.Timers.ElapsedEventHandler(timer_output_addition_1_tick);
                    //额外输出定时器计数器
                    addtional_timer_1_amount = 0;
                    //更新输出位置状态
                    update_output_state();
                    //清空输出日志
                    if (!(flag_output_position_2_in_output || flag_output_position_3_in_output || flag_output_position_4_in_output))
                    {
                        listview_log_output.Items.Clear();
                    }
                    //启动输出线程
                    MethodInvoker my_methodinvoker = new MethodInvoker(begin_thread_output_position_1_process);
                    my_methodinvoker.BeginInvoke(null, null);
                }
                else if (btn_output_1_start.Text.Trim().ToString() == "终止输出")
                {

                    //按键变为"启动输出"
                    btn_output_1_start.Text = "启动输出";

                    //控制设置可用
                    btn_output_1_control_config.Enabled = true;
                    //开机自动启动可用
                    checkbox_autorun_output_1.Enabled = true;
                    //测试输出可用
                    btn_output_1_test.Enabled = true;
                    //解锁输出组
                    tab_control_output_1.Enabled = true;
                    //重置输出中标志位
                    flag_output_position_1_in_output = false;
                    //设置文字颜色
                    lbl_trigger_position_1_indicator.ForeColor = Color.Black;
                    lbl_output_position_1_indicator.ForeColor = Color.Black;
                    lbl_output_position_1_recycle_indicator.ForeColor = Color.Black;
                    lbl_trigger_position_1_tag.ForeColor = Color.Black;
                    //更新输出位置状态
                    update_output_state();
                }

            }
            catch (Exception)
            {
                //MessageBox.Show("错误：" + ee.ToString(), "提示", MessageBoxButtons.OK);
                //错误显示
                error_message_show();
                //退出程序
                Environment.Exit(0);
            }
            finally
            {
            }

        }
        #endregion


     
        #region 控制设置
        /// <summary>
        /// 函数功能：控制设置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_output_1_control_config_Click(object sender, EventArgs e)
        {
            try
            {
                frm_control_config my_frm_control_config = new frm_control_config(this);
                my_frm_control_config.Tag = "grp_output_1";
                //设置标题
                my_frm_control_config.Text = "输出端口一控制设置";
            
                //设置窗体赋值
                
                //拍摄延时
                 my_frm_control_config.txtbox_control_config_shoot_delay.Text = 端口侦听控制系统.Properties.Settings.Default.grp_output_1_control_config_shoot_delay.ToString();
                //连拍计时器
                 my_frm_control_config.txtbox_control_config_series_shoot_timer.Text = 端口侦听控制系统.Properties.Settings.Default.grp_output_1_control_config_series_shoot_timer.ToString();
                //拍摄忽略
                 my_frm_control_config.txtbox_control_config_series_shoot_neglect.Text = 端口侦听控制系统.Properties.Settings.Default.grp_output_1_control_config_shoot_neglect.ToString ();     
                //额外触发次数
                 my_frm_control_config.txtbox_control_config_series_shoot_additional_number.Text = 端口侦听控制系统.Properties.Settings.Default.grp_output_1_control_config_additional_number.ToString ();
                //额外触发数量
                 my_frm_control_config.txtbox_control_config_series_shoot_additional_amount.Text = 端口侦听控制系统.Properties.Settings.Default.grp_output_1_control_config_additional_amount.ToString ();
                //额外触发间隔
                 my_frm_control_config.txtbox_control_config_series_shoot_additional_interval.Text = 端口侦听控制系统.Properties.Settings.Default.grp_output_1_control_config_additional_interval.ToString ();
                //弹出设置窗体        
                 my_frm_control_config.ShowDialog();

            }
            catch (Exception ee)
            {
                MessageBox.Show("错误：" + ee.ToString(), "提示", MessageBoxButtons.OK);

                //退出程序
                Environment.Exit(0);
            }
            finally
            {
            }
        }
        #endregion



        #region 通讯设置
        /// <summary>
        /// 函数功能：通讯设置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_output_1_sp_communication_config_Click(object sender, EventArgs e)
        {
            try
            {
                frm_communication_config my_frm_communication_config = new frm_communication_config(this);
                my_frm_communication_config.Tag = "grp_output_1";

                //设置窗体赋值

                //端口名
                my_frm_communication_config.txtbox_communication_config_port.Text = 端口侦听控制系统.Properties.Settings.Default.grp_output_1_port_comunication_config_port_name.ToString();
                //波特率
                my_frm_communication_config.txtbox_communication_config_baud_rate.Text = 端口侦听控制系统.Properties.Settings.Default.grp_output_1_port_comunication_config_baud_rate.ToString();
                //数据位
                my_frm_communication_config.txtbox_communication_config_databit.Text = 端口侦听控制系统.Properties.Settings.Default.grp_output_1_port_comunication_config_databit.ToString();
                //校验位
                my_frm_communication_config.cmb_communication_config_parity.Text = 端口侦听控制系统.Properties.Settings.Default.grp_output_1_port_comunication_config_parity.ToString();
                //停止位
                my_frm_communication_config.cmb_communication_config_stopbit.Text = 端口侦听控制系统.Properties.Settings.Default.grp_output_1_port_comunication_config_stopbit.ToString();

                //弹出设置窗体        
                my_frm_communication_config.ShowDialog();

            }
            catch (Exception ee)
            {
                MessageBox.Show("错误：" + ee.ToString(), "提示", MessageBoxButtons.OK);

                //退出程序
                Environment.Exit(0);
            }
            finally
            {
            }

        }
        #endregion


        #endregion



        #region 触发位置二


        #region 启动输出/终止输出
        /// <summary>
        /// 函数功能：启动输出
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_output_2_start_Click(object sender, EventArgs e)
        {
            try
            {
                if (btn_output_2_start.Text.Trim().ToString() == "启动输出")
                {
                    //端口设置
                    sp_output_position_2 = new SerialPort();

                    //端口名
                    sp_output_position_2.PortName = 端口侦听控制系统.Properties.Settings.Default.grp_output_2_port_comunication_config_port_name;
                    //波特率
                    sp_output_position_2.BaudRate = 端口侦听控制系统.Properties.Settings.Default.grp_output_2_port_comunication_config_baud_rate;
                    //数据位
                    sp_output_position_2.DataBits = 端口侦听控制系统.Properties.Settings.Default.grp_output_2_port_comunication_config_databit;
                    // 奇偶校验
                    switch (端口侦听控制系统.Properties.Settings.Default.grp_output_2_port_comunication_config_parity)
                    {
                        case "None":
                            sp_output_position_2.Parity = Parity.None;
                            break;
                        case "Odd":
                            sp_output_position_2.Parity = Parity.Odd;
                            break;
                        case "Even":
                            sp_output_position_2.Parity = Parity.Even;
                            break;
                        case "Mark:":
                            sp_output_position_2.Parity = Parity.Mark;
                            break;
                        case "Space":
                            sp_output_position_2.Parity = Parity.Space;
                            break;
                    }

                    // 停止位
                    switch (端口侦听控制系统.Properties.Settings.Default.grp_output_2_port_comunication_config_stopbit)
                    {
                        case "One":
                            sp_output_position_2.StopBits = StopBits.One;
                            break;
                        case "Two":
                            sp_output_position_2.StopBits = StopBits.Two;
                            break;
                        case "OnePointFive":
                            sp_output_position_2.StopBits = StopBits.OnePointFive;
                            break;
                    }



                    //按键变为"终止输出"
                    btn_output_2_start.Text = "终止输出";
                    //控制设置不可用
                    btn_output_2_control_config.Enabled = false;
                    //开机自动启动
                    checkbox_autorun_output_2.Enabled = false;
                    //测试输出不可用
                    btn_output_2_test.Enabled = false;
                    //锁定输出组
                    tab_control_output_2.Enabled = false;
                    //端口2触发次数统计
                    lbl_trigger_position_2_indicator.Text = "0";
                    lbl_trigger_position_2_indicator.ForeColor = Color.Blue;
                    //重置输出位置2次数计数器(用于计算总的输出次数)
                    output_position_2_output_indicator = 0;
                    lbl_output_position_2_indicator.Text = "0";
                    lbl_output_position_2_indicator.ForeColor = Color.Blue;
                    //重置输出位置2循环内触发计数器(用于计算循环内触发计数)
                    output_position_2_trigger_indicator = 0;
                    //输出位置2循环计数器(用于计算循环次数的统计)
                    output_position_2_recycle_indicator = 0;
                    lbl_output_position_2_recycle_indicator.Text = "0";
                    lbl_output_position_2_recycle_indicator.ForeColor = Color.Blue;
                    //设置位置标签颜色
                    lbl_trigger_position_2_tag.ForeColor = Color.Blue;
                    //重置触发端口2标志
                    flag_output_2_trigger = false;
                    //输出中标志
                    flag_output_position_2_in_output = true;
                    //初始化定时器与设置时间间隔
                    timer_output_2 = new System.Timers.Timer(端口侦听控制系统.Properties.Settings.Default.grp_output_2_control_config_series_shoot_timer);
                    //设置定时器事件
                    timer_output_2.Elapsed += new System.Timers.ElapsedEventHandler(timer_output_2_tick);
                    if (端口侦听控制系统.Properties.Settings.Default.grp_output_2_control_config_additional_interval > 0)
                    {
                        //初始化额外输出定时器
                        timer_output_addition_2 = new System.Timers.Timer(端口侦听控制系统.Properties.Settings.Default.grp_output_2_control_config_additional_interval);
                    }
                    else
                    {
                        //初始化额外输出定时器
                        timer_output_addition_2 = new System.Timers.Timer(10);
                    }
                    //额外输出定时器事件
                    timer_output_addition_2.Elapsed += new System.Timers.ElapsedEventHandler(timer_output_addition_2_tick);
                    //额外输出定时器计数器
                    addtional_timer_2_amount = 0;
                    //更新输出位置状态
                    update_output_state();
                    //清空输出日志
                    if (!(flag_output_position_1_in_output || flag_output_position_3_in_output || flag_output_position_4_in_output))
                    {
                        listview_log_output.Items.Clear();
                    }
                    //启动输出线程
                    MethodInvoker my_methodinvoker = new MethodInvoker(begin_thread_output_position_2_process);
                    my_methodinvoker.BeginInvoke(null, null);
                }
                else if (btn_output_2_start.Text.Trim().ToString() == "终止输出")
                {
                    //按键变为"启动输出"
                    btn_output_2_start.Text = "启动输出";
                    //控制设置可用
                    btn_output_2_control_config.Enabled = true;
                    //开机自动启动可用
                    checkbox_autorun_output_2.Enabled = true;
                    //测试输出可用
                    btn_output_2_test.Enabled = true;
                    //解锁输出组
                    tab_control_output_2.Enabled = true;
                    //重置输出中标志位
                    flag_output_position_2_in_output = false;
                    //设置文字颜色
                    lbl_trigger_position_2_indicator.ForeColor = Color.Black;
                    lbl_output_position_2_indicator.ForeColor = Color.Black;
                    lbl_output_position_2_recycle_indicator.ForeColor = Color.Black;
                    lbl_trigger_position_2_tag.ForeColor = Color.Black;
                    //更新输出位置状态
                    update_output_state();
                }

            }
            catch (Exception)
            {
                //MessageBox.Show("错误：" + ee.ToString(), "提示", MessageBoxButtons.OK);
                //错误显示
                error_message_show();
                //退出程序
                Environment.Exit(0);
            }
            finally
            {
            }

        }
        #endregion
   
  

        #region 控制设置
        /// <summary>
        /// 函数功能：控制设置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_output_2_control_config_Click(object sender, EventArgs e)
        {
            try
            {
                frm_control_config my_frm_control_config = new frm_control_config(this);
                my_frm_control_config.Tag = "grp_output_2";
                //设置标题
                my_frm_control_config.Text = "输出端口二控制设置";

                //设置窗体赋值

                //拍摄延时
                my_frm_control_config.txtbox_control_config_shoot_delay.Text = 端口侦听控制系统.Properties.Settings.Default.grp_output_2_control_config_shoot_delay.ToString();
                //连拍计时器
                my_frm_control_config.txtbox_control_config_series_shoot_timer.Text = 端口侦听控制系统.Properties.Settings.Default.grp_output_2_control_config_series_shoot_timer.ToString();
                //拍摄忽略
                my_frm_control_config.txtbox_control_config_series_shoot_neglect.Text = 端口侦听控制系统.Properties.Settings.Default.grp_output_2_control_config_shoot_neglect.ToString();
                //额外触发次数
                my_frm_control_config.txtbox_control_config_series_shoot_additional_number.Text = 端口侦听控制系统.Properties.Settings.Default.grp_output_2_control_config_additional_number.ToString();
                //额外触发数量
                my_frm_control_config.txtbox_control_config_series_shoot_additional_amount.Text = 端口侦听控制系统.Properties.Settings.Default.grp_output_2_control_config_additional_amount.ToString();
                //额外触发间隔
                my_frm_control_config.txtbox_control_config_series_shoot_additional_interval.Text = 端口侦听控制系统.Properties.Settings.Default.grp_output_2_control_config_additional_interval.ToString();
                //弹出设置窗体        
                my_frm_control_config.ShowDialog();

            }
            catch (Exception ee)
            {
                MessageBox.Show("错误：" + ee.ToString(), "提示", MessageBoxButtons.OK);

                //退出程序
                Environment.Exit(0);
            }
            finally
            {
            }

        }
        #endregion
    


        #region 通讯设置
        /// <summary>
        /// 函数功能：通讯设置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_output_2_sp_communication_config_Click(object sender, EventArgs e)
        {
            try
            {
                frm_communication_config my_frm_communication_config = new frm_communication_config(this);
                my_frm_communication_config.Tag = "grp_output_2";

                //设置窗体赋值

                //端口名
                my_frm_communication_config.txtbox_communication_config_port.Text = 端口侦听控制系统.Properties.Settings.Default.grp_output_2_port_comunication_config_port_name.ToString();
                //波特率
                my_frm_communication_config.txtbox_communication_config_baud_rate.Text = 端口侦听控制系统.Properties.Settings.Default.grp_output_2_port_comunication_config_baud_rate.ToString();
                //数据位
                my_frm_communication_config.txtbox_communication_config_databit.Text = 端口侦听控制系统.Properties.Settings.Default.grp_output_2_port_comunication_config_databit.ToString();
                //校验位
                my_frm_communication_config.cmb_communication_config_parity.Text = 端口侦听控制系统.Properties.Settings.Default.grp_output_2_port_comunication_config_parity.ToString();
                //停止位
                my_frm_communication_config.cmb_communication_config_stopbit.Text = 端口侦听控制系统.Properties.Settings.Default.grp_output_2_port_comunication_config_stopbit.ToString();

                //弹出设置窗体        
                my_frm_communication_config.ShowDialog();

            }
            catch (Exception ee)
            {
                MessageBox.Show("错误：" + ee.ToString(), "提示", MessageBoxButtons.OK);

                //退出程序
                Environment.Exit(0);
            }
            finally
            {
            }

        }
        #endregion


        #endregion



        #region 触发位置三


        #region 启动输出/终止输出
        /// <summary>
        /// 函数功能：启动输出
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_output_3_start_Click(object sender, EventArgs e)
        {
            try
            {
                if (btn_output_3_start.Text.Trim().ToString() == "启动输出")
                {
                    //端口设置
                    sp_output_position_3 = new SerialPort();

                    //端口名
                    sp_output_position_3.PortName = 端口侦听控制系统.Properties.Settings.Default.grp_output_3_port_comunication_config_port_name;
                    //波特率
                    sp_output_position_3.BaudRate = 端口侦听控制系统.Properties.Settings.Default.grp_output_3_port_comunication_config_baud_rate;
                    //数据位
                    sp_output_position_3.DataBits = 端口侦听控制系统.Properties.Settings.Default.grp_output_3_port_comunication_config_databit;
                    // 奇偶校验
                    switch (端口侦听控制系统.Properties.Settings.Default.grp_output_3_port_comunication_config_parity)
                    {
                        case "None":
                            sp_output_position_3.Parity = Parity.None;
                            break;
                        case "Odd":
                            sp_output_position_3.Parity = Parity.Odd;
                            break;
                        case "Even":
                            sp_output_position_3.Parity = Parity.Even;
                            break;
                        case "Mark:":
                            sp_output_position_3.Parity = Parity.Mark;
                            break;
                        case "Space":
                            sp_output_position_3.Parity = Parity.Space;
                            break;
                    }

                    // 停止位
                    switch (端口侦听控制系统.Properties.Settings.Default.grp_output_3_port_comunication_config_stopbit)
                    {
                        case "One":
                            sp_output_position_3.StopBits = StopBits.One;
                            break;
                        case "Two":
                            sp_output_position_3.StopBits = StopBits.Two;
                            break;
                        case "OnePointFive":
                            sp_output_position_3.StopBits = StopBits.OnePointFive;
                            break;
                    }


                    //按键变为"终止输出"
                    btn_output_3_start.Text = "终止输出";
                    //控制设置不可用
                    btn_output_3_control_config.Enabled = false;
                    //开机自动启动
                    checkbox_autorun_output_3.Enabled = false;
                    //测试输出不可用
                    btn_output_3_test.Enabled = false;
                    //锁定输出组
                    tab_control_output_3.Enabled = false;
                    //端口3触发次数统计
                    lbl_trigger_position_3_indicator.Text = "0";
                    lbl_trigger_position_3_indicator.ForeColor = Color.Blue;
                    //重置输出位置3次数计数器(用于计算总的输出次数)
                    output_position_3_output_indicator = 0;
                    lbl_output_position_3_indicator.Text = "0";
                    lbl_output_position_3_indicator.ForeColor = Color.Blue;
                    //重置输出位置3循环内触发计数器(用于计算循环内触发计数)
                    output_position_3_trigger_indicator = 0;
                    //输出位置3循环计数器(用于计算循环次数的统计)
                    output_position_3_recycle_indicator = 0;
                    lbl_output_position_3_recycle_indicator.Text = "0";
                    lbl_output_position_3_recycle_indicator.ForeColor = Color.Blue;
                    //设置位置标签颜色
                    lbl_trigger_position_3_tag.ForeColor = Color.Blue;
                    //重置触发端口3标志
                    flag_output_3_trigger = false;
                    //输出中标志
                    flag_output_position_3_in_output = true;
                    //初始化定时器与设置时间间隔
                    timer_output_3 = new System.Timers.Timer(端口侦听控制系统.Properties.Settings.Default.grp_output_3_control_config_series_shoot_timer);
                    //设置定时器事件
                    timer_output_3.Elapsed += new System.Timers.ElapsedEventHandler(timer_output_3_tick);
                    if (端口侦听控制系统.Properties.Settings.Default.grp_output_3_control_config_additional_interval > 0)
                    {
                        //初始化额外输出定时器
                        timer_output_addition_3 = new System.Timers.Timer(端口侦听控制系统.Properties.Settings.Default.grp_output_3_control_config_additional_interval);
                    }
                    else
                    {
                        //初始化额外输出定时器
                        timer_output_addition_3 = new System.Timers.Timer(10);
                    }
                    //额外输出定时器事件
                    timer_output_addition_3.Elapsed += new System.Timers.ElapsedEventHandler(timer_output_addition_3_tick);
                    //额外输出定时器计数器
                    addtional_timer_3_amount = 0;
                    //更新输出位置状态
                    update_output_state();
                    //清空输出日志
                    if (!(flag_output_position_1_in_output || flag_output_position_2_in_output || flag_output_position_4_in_output))
                    {
                        listview_log_output.Items.Clear();
                    }
                    //启动输出线程
                    MethodInvoker my_methodinvoker = new MethodInvoker(begin_thread_output_position_3_process);
                    my_methodinvoker.BeginInvoke(null, null);
                }
                else if (btn_output_3_start.Text.Trim().ToString() == "终止输出")
                {
                    //按键变为"启动输出"
                    btn_output_3_start.Text = "启动输出";
                    //控制设置可用
                    btn_output_3_control_config.Enabled = true;
                    //开机自动启动可用
                    checkbox_autorun_output_3.Enabled = true;
                    //测试输出可用
                    btn_output_3_test.Enabled = true;
                    //解锁输出组
                    tab_control_output_3.Enabled = true;
                    //重置输出中标志位
                    flag_output_position_3_in_output = false;
                    //设置文字颜色
                    lbl_trigger_position_3_indicator.ForeColor = Color.Black;
                    lbl_output_position_3_indicator.ForeColor = Color.Black;
                    lbl_output_position_3_recycle_indicator.ForeColor = Color.Black;
                    lbl_trigger_position_3_tag.ForeColor = Color.Black;
                    //更新输出位置状态
                    update_output_state();
                }

            }
            catch (Exception)
            {
                //MessageBox.Show("错误：" + ee.ToString(), "提示", MessageBoxButtons.OK);
                //错误显示
                error_message_show();
                //退出程序
                Environment.Exit(0);
            }
            finally
            {
            }

        }

        #endregion

     

        #region 控制设置
        /// <summary>
        /// 函数功能：控制设置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_output_3_control_config_Click(object sender, EventArgs e)
        {
            try
            {
                frm_control_config my_frm_control_config = new frm_control_config(this);
                my_frm_control_config.Tag = "grp_output_3";
                //设置标题
                my_frm_control_config.Text = "输出端口三控制设置";

                //设置窗体赋值

                //拍摄延时
                my_frm_control_config.txtbox_control_config_shoot_delay.Text = 端口侦听控制系统.Properties.Settings.Default.grp_output_3_control_config_shoot_delay.ToString();
                //连拍计时器
                my_frm_control_config.txtbox_control_config_series_shoot_timer.Text = 端口侦听控制系统.Properties.Settings.Default.grp_output_3_control_config_series_shoot_timer.ToString();
                //拍摄忽略
                my_frm_control_config.txtbox_control_config_series_shoot_neglect.Text = 端口侦听控制系统.Properties.Settings.Default.grp_output_3_control_config_shoot_neglect.ToString();
                //额外触发次数
                my_frm_control_config.txtbox_control_config_series_shoot_additional_number.Text = 端口侦听控制系统.Properties.Settings.Default.grp_output_3_control_config_additional_number.ToString();
                //额外触发数量
                my_frm_control_config.txtbox_control_config_series_shoot_additional_amount.Text = 端口侦听控制系统.Properties.Settings.Default.grp_output_3_control_config_additional_amount.ToString();
                //额外触发间隔
                my_frm_control_config.txtbox_control_config_series_shoot_additional_interval.Text = 端口侦听控制系统.Properties.Settings.Default.grp_output_3_control_config_additional_interval.ToString();
                //弹出设置窗体        
                my_frm_control_config.ShowDialog();

            }
            catch (Exception ee)
            {
                MessageBox.Show("错误：" + ee.ToString(), "提示", MessageBoxButtons.OK);

                //退出程序
                Environment.Exit(0);
            }
            finally
            {
            }

        }
        #endregion


    
        #region 通讯设置
        /// <summary>
        /// 函数功能：通讯设置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_output_3_sp_communication_config_Click(object sender, EventArgs e)
        {
            try
            {
                frm_communication_config my_frm_communication_config = new frm_communication_config(this);
                my_frm_communication_config.Tag = "grp_output_3";

                //设置窗体赋值

                //端口名
                my_frm_communication_config.txtbox_communication_config_port.Text = 端口侦听控制系统.Properties.Settings.Default.grp_output_3_port_comunication_config_port_name.ToString();
                //波特率
                my_frm_communication_config.txtbox_communication_config_baud_rate.Text = 端口侦听控制系统.Properties.Settings.Default.grp_output_3_port_comunication_config_baud_rate.ToString();
                //数据位
                my_frm_communication_config.txtbox_communication_config_databit.Text = 端口侦听控制系统.Properties.Settings.Default.grp_output_3_port_comunication_config_databit.ToString();
                //校验位
                my_frm_communication_config.cmb_communication_config_parity.Text = 端口侦听控制系统.Properties.Settings.Default.grp_output_3_port_comunication_config_parity.ToString();
                //停止位
                my_frm_communication_config.cmb_communication_config_stopbit.Text = 端口侦听控制系统.Properties.Settings.Default.grp_output_3_port_comunication_config_stopbit.ToString();

                //弹出设置窗体        
                my_frm_communication_config.ShowDialog();

            }
            catch (Exception ee)
            {
                MessageBox.Show("错误：" + ee.ToString(), "提示", MessageBoxButtons.OK);

                //退出程序
                Environment.Exit(0);
            }
            finally
            {
            }

        }

        #endregion


        #endregion



        #region 触发位置四


        #region 启动输出/终止输出
        /// <summary>
        /// 函数功能：启动输出
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_output_4_start_Click(object sender, EventArgs e)
        {
            try
            {
                if (btn_output_4_start.Text.Trim().ToString() == "启动输出")
                {
                    //端口设置
                    sp_output_position_4 = new SerialPort();

                    //端口名
                    sp_output_position_4.PortName = 端口侦听控制系统.Properties.Settings.Default.grp_output_4_port_comunication_config_port_name;
                    //波特率
                    sp_output_position_4.BaudRate = 端口侦听控制系统.Properties.Settings.Default.grp_output_4_port_comunication_config_baud_rate;
                    //数据位
                    sp_output_position_4.DataBits = 端口侦听控制系统.Properties.Settings.Default.grp_output_4_port_comunication_config_databit;
                    // 奇偶校验
                    switch (端口侦听控制系统.Properties.Settings.Default.grp_output_4_port_comunication_config_parity)
                    {
                        case "None":
                            sp_output_position_4.Parity = Parity.None;
                            break;
                        case "Odd":
                            sp_output_position_4.Parity = Parity.Odd;
                            break;
                        case "Even":
                            sp_output_position_4.Parity = Parity.Even;
                            break;
                        case "Mark:":
                            sp_output_position_4.Parity = Parity.Mark;
                            break;
                        case "Space":
                            sp_output_position_4.Parity = Parity.Space;
                            break;
                    }

                    // 停止位
                    switch (端口侦听控制系统.Properties.Settings.Default.grp_output_4_port_comunication_config_stopbit)
                    {
                        case "One":
                            sp_output_position_4.StopBits = StopBits.One;
                            break;
                        case "Two":
                            sp_output_position_4.StopBits = StopBits.Two;
                            break;
                        case "OnePointFive":
                            sp_output_position_4.StopBits = StopBits.OnePointFive;
                            break;
                    }


                    //按键变为"终止输出"
                    btn_output_4_start.Text = "终止输出";
                    //控制设置不可用
                    btn_output_4_control_config.Enabled = false;
                    //开机自动启动
                    checkbox_autorun_output_4.Enabled = false;
                    //测试输出不可用
                    btn_output_4_test.Enabled = false;
                    //锁定输出组
                    tab_control_output_4.Enabled = false;
                    //端口4触发次数统计
                    lbl_trigger_position_4_indicator.Text = "0";
                    lbl_trigger_position_4_indicator.ForeColor = Color.Blue;
                    //重置输出位置4次数计数器(用于计算总的输出次数)
                    output_position_4_output_indicator = 0;
                    lbl_output_position_4_indicator.Text = "0";
                    lbl_output_position_4_indicator.ForeColor = Color.Blue;
                    //重置输出位置4循环内触发计数器(用于计算循环内触发计数)
                    output_position_4_trigger_indicator = 0;
                    //输出位置4循环计数器(用于计算循环次数的统计)
                    output_position_4_recycle_indicator = 0;
                    lbl_output_position_4_recycle_indicator.Text = "0";
                    lbl_output_position_4_recycle_indicator.ForeColor = Color.Blue;
                    //设置位置标签颜色
                    lbl_trigger_position_4_tag.ForeColor = Color.Blue;
                    //重置触发端口4标志
                    flag_output_4_trigger = false;
                    //输出中标志
                    flag_output_position_4_in_output = true;
                    //初始化定时器与设置时间间隔
                    timer_output_4 = new System.Timers.Timer(端口侦听控制系统.Properties.Settings.Default.grp_output_4_control_config_series_shoot_timer);
                    //设置定时器事件
                    timer_output_4.Elapsed += new System.Timers.ElapsedEventHandler(timer_output_4_tick);
                    if (端口侦听控制系统.Properties.Settings.Default.grp_output_4_control_config_additional_interval > 0)
                    {
                        //初始化额外输出定时器
                        timer_output_addition_4 = new System.Timers.Timer(端口侦听控制系统.Properties.Settings.Default.grp_output_4_control_config_additional_interval);
                    }
                    else
                    {
                        //初始化额外输出定时器
                        timer_output_addition_4 = new System.Timers.Timer(10);
                    }
                    //额外输出定时器事件
                    timer_output_addition_4.Elapsed += new System.Timers.ElapsedEventHandler(timer_output_addition_4_tick);
                    //额外输出定时器计数器
                    addtional_timer_4_amount = 0;
                    //更新输出位置状态
                    update_output_state();
                    //清空输出日志
                    if (!(flag_output_position_1_in_output || flag_output_position_2_in_output || flag_output_position_3_in_output))
                    {
                        listview_log_output.Items.Clear();
                    }
                    //启动输出线程
                    MethodInvoker my_methodinvoker = new MethodInvoker(begin_thread_output_position_4_process);
                    my_methodinvoker.BeginInvoke(null, null);
                }
                else if (btn_output_4_start.Text.Trim().ToString() == "终止输出")
                {

                    //按键变为"启动输出"
                    btn_output_4_start.Text = "启动输出";
                    //控制设置可用
                    btn_output_4_control_config.Enabled = true;
                    //开机自动启动可用
                    checkbox_autorun_output_4.Enabled = true;
                    //测试输出可用
                    btn_output_4_test.Enabled = true;
                    //解锁输出组
                    tab_control_output_4.Enabled = true;
                    //重置输出中标志位
                    flag_output_position_4_in_output = false;
                    //设置文字颜色
                    lbl_trigger_position_4_indicator.ForeColor = Color.Black;
                    lbl_output_position_4_indicator.ForeColor = Color.Black;
                    lbl_output_position_4_recycle_indicator.ForeColor = Color.Black;
                    lbl_trigger_position_4_tag.ForeColor = Color.Black;
                    //更新输出位置状态
                    update_output_state();
                }
            }
            catch (Exception)
            {
                //MessageBox.Show("错误：" + ee.ToString(), "提示", MessageBoxButtons.OK);
                //错误显示
                error_message_show();
                //退出程序
                Environment.Exit(0);
            }
            finally
            {
            }

        }
        #endregion
   


        #region 控制设置
        /// <summary>
        /// 函数功能：控制设置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_output_4_control_config_Click(object sender, EventArgs e)
        {
            try
            {
                frm_control_config my_frm_control_config = new frm_control_config(this);
                my_frm_control_config.Tag = "grp_output_4";
                //设置标题
                my_frm_control_config.Text = "输出端口四控制设置";

                //设置窗体赋值

                //拍摄延时
                my_frm_control_config.txtbox_control_config_shoot_delay.Text = 端口侦听控制系统.Properties.Settings.Default.grp_output_4_control_config_shoot_delay.ToString();
                //连拍计时器
                my_frm_control_config.txtbox_control_config_series_shoot_timer.Text = 端口侦听控制系统.Properties.Settings.Default.grp_output_4_control_config_series_shoot_timer.ToString();
                //拍摄忽略
                my_frm_control_config.txtbox_control_config_series_shoot_neglect.Text = 端口侦听控制系统.Properties.Settings.Default.grp_output_4_control_config_shoot_neglect.ToString();
                //额外触发次数
                my_frm_control_config.txtbox_control_config_series_shoot_additional_number.Text = 端口侦听控制系统.Properties.Settings.Default.grp_output_4_control_config_additional_number.ToString();
                //额外触发数量
                my_frm_control_config.txtbox_control_config_series_shoot_additional_amount.Text = 端口侦听控制系统.Properties.Settings.Default.grp_output_4_control_config_additional_amount.ToString();
                //额外触发间隔
                my_frm_control_config.txtbox_control_config_series_shoot_additional_interval.Text = 端口侦听控制系统.Properties.Settings.Default.grp_output_4_control_config_additional_interval.ToString();
                //弹出设置窗体        
                my_frm_control_config.ShowDialog();

            }
            catch (Exception ee)
            {
                MessageBox.Show("错误：" + ee.ToString(), "提示", MessageBoxButtons.OK);

                //退出程序
                Environment.Exit(0);
            }
            finally
            {
            }

        }
        #endregion



        #region 通讯设置
        /// <summary>
        /// 函数功能：通讯设置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_output_4_sp_communication_config_Click(object sender, EventArgs e)
        {
            try
            {
                frm_communication_config my_frm_communication_config = new frm_communication_config(this);
                my_frm_communication_config.Tag = "grp_output_4";

                //设置窗体赋值

                //端口名
                my_frm_communication_config.txtbox_communication_config_port.Text = 端口侦听控制系统.Properties.Settings.Default.grp_output_4_port_comunication_config_port_name.ToString();
                //波特率
                my_frm_communication_config.txtbox_communication_config_baud_rate.Text = 端口侦听控制系统.Properties.Settings.Default.grp_output_4_port_comunication_config_baud_rate.ToString();
                //数据位
                my_frm_communication_config.txtbox_communication_config_databit.Text = 端口侦听控制系统.Properties.Settings.Default.grp_output_4_port_comunication_config_databit.ToString();
                //校验位
                my_frm_communication_config.cmb_communication_config_parity.Text = 端口侦听控制系统.Properties.Settings.Default.grp_output_4_port_comunication_config_parity.ToString();
                //停止位
                my_frm_communication_config.cmb_communication_config_stopbit.Text = 端口侦听控制系统.Properties.Settings.Default.grp_output_4_port_comunication_config_stopbit.ToString();

                //弹出设置窗体        
                my_frm_communication_config.ShowDialog();

            }
            catch (Exception ee)
            {
                MessageBox.Show("错误：" + ee.ToString(), "提示", MessageBoxButtons.OK);

                //退出程序
                Environment.Exit(0);
            }
            finally
            {
            }

        }
        #endregion     



        #endregion



        #endregion


        #region 保存输出界面设置
        /// <summary>
        /// 函数功能：保存输出界面设置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_output_save_Click(object sender, EventArgs e)
        {
            try
            {
                //检查
                //显示错误提示控件
                ToolTip my_tooltip = new ToolTip();
                //字符串
                string[] temp = null;
                //输出数字
                int out_result = 0;

                //端口一

                //检测端口信息
                //如果端口信息不为空
                if (txtbox_output_1_sp_write_data.Text.Trim().Length > 0)
                {
                    //触发位置一写入内容为（0-255字符串）
                    //字符串
                    temp = null;
                    //输出数字
                    out_result = 0;
                    temp = txtbox_output_1_sp_write_data.Text.Trim().Split(' ');
                    for (int i = 0; i < temp.Length; i++)
                    {
                        //检查是否为数字
                        if (!int.TryParse(temp[i], out out_result))
                        {
                            //跳转到当前页
                            tab_control_output_1.SelectedTab = tab_control_output_1.TabPages[0];
                            //设置输入焦点
                            txtbox_output_1_sp_write_data.Focus();
                            my_tooltip.Show("输入字节必须为0-255之间的数字，字节间用单个空格分隔", txtbox_output_1_sp_write_data, 3000);                        

                            return;
                        }
                        //检查是否为0-255之间的数字
                        if (out_result < 0 && out_result > 255)
                        {
                            //跳转到当前页
                            tab_control_output_1.SelectedTab = tab_control_output_1.TabPages[0];
                            //设置输入焦点
                            txtbox_output_1_sp_write_data.Focus();
                            my_tooltip.Show("输入字节必须为0-255之间的数字，字节间用单个空格分隔", txtbox_output_1_sp_write_data, 3000);                        

                            return;
                        }
                       
                    }
                }

                //检测window消息发送内容
                if (txtbox_output_1_window_data_2.Text.Trim().Length > 0)
                {
                    out_result = 0;

                    if (!int.TryParse(txtbox_output_1_window_data_2 .Text .Trim ().ToString (),out out_result))
                    {
                        //跳转到当前页
                        tab_control_output_1.SelectedTab = tab_control_output_1.TabPages[2];
                        //设为输入焦点
                        txtbox_output_1_window_data_2.Focus();
                        my_tooltip.Show("消息内容必须为数字", txtbox_output_1_window_data_2, 3000);                     

                        return;
                    }
                }

                //端口二
                //检测端口信息

                //如果端口信息不为空
                if (txtbox_output_2_sp_write_data.Text.Trim().Length > 0)
                {
                    //触发位置二写入内容为（0-255字符串）
                    //字符串
                    temp = null;
                    //输出数字
                    out_result = 0;
                    temp = txtbox_output_2_sp_write_data.Text.Trim().Split(' ');
                    for (int i = 0; i < temp.Length; i++)
                    {
                        //检查是否为数字
                        
                        if (!int.TryParse(temp[i], out out_result))
                        {
                            //跳转到当前页
                            tab_control_output_2.SelectedTab = tab_control_output_2.TabPages[0];
                            //设置输入焦点
                            txtbox_output_2_sp_write_data.Focus();
                            my_tooltip.Show("输入字节必须为0-255之间的数字，字节间用单个空格分隔", txtbox_output_2_sp_write_data, 3000);                        

                            return;
                        }
                        //检查是否为0-255之间的数字
                        if (out_result < 0 && out_result > 255)
                        {
                            //跳转到当前页
                            tab_control_output_2.SelectedTab = tab_control_output_2.TabPages[0];
                            //设置输入焦点
                            txtbox_output_2_sp_write_data.Focus();
                            my_tooltip.Show("输入字节必须为0-255之间的数字，字节间用单个空格分隔", txtbox_output_2_sp_write_data, 3000);                        

                            return;
                        }

                    }
                }


                //检测window消息发送内容
                if (txtbox_output_2_window_data_2.Text.Trim().Length > 0)
                {
                    out_result = 0;

                    if (!int.TryParse(txtbox_output_2_window_data_2.Text.Trim().ToString(), out out_result))
                    {
                        //跳转到当前页
                        tab_control_output_2.SelectedTab = tab_control_output_2.TabPages[2];
                        //设为输入焦点
                        txtbox_output_2_window_data_2.Focus();
                        my_tooltip.Show("消息内容必须为数字", txtbox_output_2_window_data_2, 3000);

                        return;
                    }
                }


                //端口三
                //检测端口信息

                //如果端口信息不为空
                if (txtbox_output_3_sp_write_data.Text.Trim().Length > 0)
                {
                    //触发位置三写入内容为（0-255字符串）
                    //字符串
                    temp = null;
                    //输出数字
                    out_result = 0;
                    temp = txtbox_output_3_sp_write_data.Text.Trim().Split(' ');
                    for (int i = 0; i < temp.Length; i++)
                    {
                        //检查是否为数字
                        if (!int.TryParse(temp[i], out out_result))
                        {
                            //跳转到当前页
                            tab_control_output_3.SelectedTab = tab_control_output_3.TabPages[0];
                            //设置输入焦点
                            txtbox_output_3_sp_write_data.Focus();
                            my_tooltip.Show("输入字节必须为0-255之间的数字，字节间用单个空格分隔", txtbox_output_3_sp_write_data, 3000);                        

                            return;
                        }
                        //检查是否为0-255之间的数字
                        if (out_result < 0 && out_result > 255)
                        {
                            //跳转到当前页
                            tab_control_output_3.SelectedTab = tab_control_output_3.TabPages[0];
                            //设置输入焦点
                            txtbox_output_3_sp_write_data.Focus();
                            my_tooltip.Show("输入字节必须为0-255之间的数字，字节间用单个空格分隔", txtbox_output_3_sp_write_data, 3000);                        

                            return;
                        }

                    }
                }

                //检测window消息发送内容
                if (txtbox_output_3_window_data_2.Text.Trim().Length > 0)
                {
                    out_result = 0;

                    if (!int.TryParse(txtbox_output_3_window_data_2.Text.Trim().ToString(), out out_result))
                    {
                        //跳转到当前页
                        tab_control_output_3.SelectedTab = tab_control_output_3.TabPages[2];
                        //设为输入焦点
                        txtbox_output_3_window_data_2.Focus();
                        my_tooltip.Show("消息内容必须为数字", txtbox_output_3_window_data_2, 3000);

                        return;
                    }
                }


                //端口四
                //检测端口信息

                //如果端口信息不为空
                if (txtbox_output_4_sp_write_data.Text.Trim().Length > 0)
                {
                    //触发位置四写入内容为（0-255字符串）
                    //字符串
                    temp = null;
                    //输出数字
                    out_result = 0;
                    temp = txtbox_output_4_sp_write_data.Text.Trim().Split(' ');
                    for (int i = 0; i < temp.Length; i++)
                    {
                        //检查是否为数字
                        if (!int.TryParse(temp[i], out out_result))
                        {
                            //跳转到当前页
                            tab_control_output_4.SelectedTab = tab_control_output_4.TabPages[0];
                            //设置输入焦点
                            txtbox_output_4_sp_write_data.Focus();
                            my_tooltip.Show("输入字节必须为0-255之间的数字，字节间用单个空格分隔", txtbox_output_4_sp_write_data, 3000);

                            return;
                        }
                        //检查是否为0-255之间的数字
                        if (out_result < 0 && out_result > 255)
                        {
                            //跳转到当前页
                            tab_control_output_4.SelectedTab = tab_control_output_4.TabPages[0];
                            //设置输入焦点
                            txtbox_output_4_sp_write_data.Focus();
                            my_tooltip.Show("输入字节必须为0-255之间的数字，字节间用单个空格分隔", txtbox_output_4_sp_write_data, 3000);

                            return;
                        }

                    }
                }

                //检测window消息发送内容
                if (txtbox_output_4_window_data_2.Text.Trim().Length > 0)
                {
                    out_result = 0;

                    if (!int.TryParse(txtbox_output_4_window_data_2.Text.Trim().ToString(), out out_result))
                    {
                        //跳转到当前页
                        tab_control_output_4.SelectedTab = tab_control_output_4.TabPages[2];
                        //设为输入焦点
                        txtbox_output_4_window_data_2.Focus();
                        my_tooltip.Show("消息内容必须为数字", txtbox_output_4_window_data_2, 3000);

                        return;
                    }
                }


                //释放错误提示控件资源
                my_tooltip.Dispose();

                //触发位置一

                //端口方式
                //输出方式
                端口侦听控制系统.Properties.Settings.Default.grp_output_1_type = tab_control_output_1.SelectedIndex;                
                //写入内容
                端口侦听控制系统.Properties.Settings.Default.grp_output_1_port_write_data = txtbox_output_1_sp_write_data.Text.Trim().ToString();

                //按键方式
                //发送内容1
                端口侦听控制系统.Properties.Settings.Default.grp_output_1_message_data_1  = txtbox_output_1_message_data_1.Text.Trim().ToString();
                //发送内容2
                端口侦听控制系统.Properties.Settings.Default.grp_output_1_message_data_2 = txtbox_output_1_message_data_2.Text.Trim().ToString();
              
                //消息方式
                //消息内容一
                端口侦听控制系统.Properties.Settings.Default.grp_output_1_window_data_1 = txtbox_output_1_window_data_1.Text.Trim().ToString();
                //消息内容二
                端口侦听控制系统.Properties.Settings.Default.grp_output_1_window_data_2 = txtbox_output_1_window_data_2.Text.Trim().ToString();
             
                //触发位置二

                //端口方式
                //输出方式
                端口侦听控制系统.Properties.Settings.Default.grp_output_2_type = tab_control_output_2.SelectedIndex;           
                //写入内容
                端口侦听控制系统.Properties.Settings.Default.grp_output_2_port_write_data = txtbox_output_2_sp_write_data.Text.Trim().ToString();
                
                //按键方式
                //发送内容1
                端口侦听控制系统.Properties.Settings.Default.grp_output_2_message_data_1 = txtbox_output_2_message_data_1.Text.Trim().ToString();
                //发送内容2
                端口侦听控制系统.Properties.Settings.Default.grp_output_2_message_data_2 = txtbox_output_2_message_data_2.Text.Trim().ToString();

                //消息方式
                //消息内容一
                端口侦听控制系统.Properties.Settings.Default.grp_output_2_window_data_1 = txtbox_output_2_window_data_1.Text.Trim().ToString();
                //消息内容二
                端口侦听控制系统.Properties.Settings.Default.grp_output_2_window_data_2 = txtbox_output_2_window_data_2.Text.Trim().ToString();
             

                //触发位置三

                //端口方式
                //输出方式
                端口侦听控制系统.Properties.Settings.Default.grp_output_3_type = tab_control_output_3.SelectedIndex;
                //写入内容
                端口侦听控制系统.Properties.Settings.Default.grp_output_3_port_write_data = txtbox_output_3_sp_write_data.Text.Trim().ToString();

                //按键方式
                //发送内容1
                端口侦听控制系统.Properties.Settings.Default.grp_output_3_message_data_1 = txtbox_output_3_message_data_1.Text.Trim().ToString();
                //发送内容2
                端口侦听控制系统.Properties.Settings.Default.grp_output_3_message_data_2 = txtbox_output_3_message_data_2.Text.Trim().ToString();
               
                //消息方式
                //消息内容一
                端口侦听控制系统.Properties.Settings.Default.grp_output_3_window_data_1 = txtbox_output_3_window_data_1.Text.Trim().ToString();
                //消息内容二
                端口侦听控制系统.Properties.Settings.Default.grp_output_3_window_data_2 = txtbox_output_3_window_data_2.Text.Trim().ToString();
             

                //触发位置四

                //端口方式
                //输出方式
                端口侦听控制系统.Properties.Settings.Default.grp_output_4_type = tab_control_output_4.SelectedIndex;
                //写入内容
                端口侦听控制系统.Properties.Settings.Default.grp_output_4_port_write_data = txtbox_output_4_sp_write_data.Text.Trim().ToString();

                //按键方式
                //发送内容1
                端口侦听控制系统.Properties.Settings.Default.grp_output_4_message_data_1 = txtbox_output_4_message_data_1.Text.Trim().ToString();
                //发送内容2
                端口侦听控制系统.Properties.Settings.Default.grp_output_4_message_data_2 = txtbox_output_4_message_data_2.Text.Trim().ToString();
             
                //消息方式
                //消息内容一
                端口侦听控制系统.Properties.Settings.Default.grp_output_4_window_data_1 = txtbox_output_4_window_data_1.Text.Trim().ToString();
                //消息内容二
                端口侦听控制系统.Properties.Settings.Default.grp_output_4_window_data_2 = txtbox_output_4_window_data_2.Text.Trim().ToString();
             

                //保存设置
                端口侦听控制系统.Properties.Settings.Default.Save();


                //更新输出位置状态
                update_output_state();

                //改变保持按键颜色
                btn_output_save.ForeColor = Color.Black;

                //弹出对话框
                MessageBox.Show("输出位置的界面参数已经保存！", "提示", MessageBoxButtons.OK);


             }
            catch (Exception ee)
            {
                MessageBox.Show("错误：" + ee.ToString(), "提示", MessageBoxButtons.OK);

                //退出程序
                Environment.Exit(0);
            }
            finally
            {
            }
        }
        #endregion


        #region 模拟触发
        /// <summary>
        /// 按键功能：模拟触发
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_start_simulate_Click(object sender, EventArgs e)
        {
            try
            {
                //模拟触发
                if (btn_start_simulate.Text.Trim().ToString() == "开始模拟")
                {

                    //检测输入值

                    //错误显示控件
                    ToolTip my_tooltip = new ToolTip();
                    //触发间隔
                    int interval = 0;

                    //检测触发间隔是否为数字
                    if (!int.TryParse(txtbox_simulate_interval.Text.Trim().ToString(), out interval))
                    {
                        //设为输入焦点
                        txtbox_simulate_interval.Focus();
                        my_tooltip.Show("触发间隔必须为整数", txtbox_simulate_interval, 3000);
                        return;
                    }

                    //检测触发间隔是否大于1000
                    if (interval < 1000)
                    {
                        //设为输入焦点
                        txtbox_simulate_interval.Focus();
                        my_tooltip.Show("触发间隔必须大于1000", txtbox_simulate_interval, 3000);
                        return;
                    }

                    //开始模拟
                    //设定按键

                    //模拟间隔
                    txtbox_simulate_interval.Enabled = false;

                    //标签
                    lbl_simulate_interval.ForeColor = Color.Blue;
                    lbl_msel.ForeColor = Color.Blue;

                    //模拟按键
                    btn_start_simulate.Text = "终止模拟";

                    //触发位置1勾选项
                    checkbox_simlate_position_1.Enabled = false;
                    //触发位置2勾选项
                    checkbox_simlate_position_2.Enabled = false;
                    //触发位置3勾选项
                    checkbox_simlate_position_3.Enabled = false;
                    //触发位置4勾选项
                    checkbox_simlate_position_4.Enabled = false;
           
                    //开始侦听变为不可用
                    btn_listening_start.Enabled = false;
                    //参数设置

                    //模拟触发中标志
                    flag_in_simulate = true;

                    //初始化listview_log_listen 
                    listview_log_listen.Items.Clear();

                    //计时器
                    timer_simulate = new System.Timers.Timer(interval);
                    timer_simulate.Elapsed += new System.Timers.ElapsedEventHandler(timer_simulate_tick);
                    //开始计数
                    timer_simulate.Start();                 


                }
                else if (btn_start_simulate.Text.Trim().ToString() == "终止模拟")
                {
                    //终止模拟
                    //设定按键

                    //模拟间隔
                    txtbox_simulate_interval.Enabled = true;

                    //标签
                    lbl_simulate_interval.ForeColor = Color.Black;
                    lbl_msel.ForeColor = Color.Black ;

                    //模拟按键
                    btn_start_simulate.Text = "开始模拟";

                    //触发位置1勾选项
                    checkbox_simlate_position_1.Enabled = true;
                    //触发位置2勾选项
                    checkbox_simlate_position_2.Enabled = true;
                    //触发位置3勾选项
                    checkbox_simlate_position_3.Enabled = true;
                    //触发位置4勾选项
                    checkbox_simlate_position_4.Enabled = true;

                    //开始侦听变为可用
                    btn_listening_start.Enabled = true;
                    //设定参数

                    //模拟触发中标志
                    flag_in_simulate = false;   
                    //计时器终止
                    timer_simulate.Stop();
                    //释放资源
                    timer_simulate.Dispose();
                }
            }
            catch (Exception)
            {
                //MessageBox.Show("错误：" + ee.ToString(), "提示", MessageBoxButtons.OK);
                //错误显示
                error_message_show();
                //退出程序
                Environment.Exit(0);
            }
            finally
            {
            }
        }
        #endregion


        #endregion


        #region 右键菜单


        #region  导出侦听日志
        /// <summary>
        /// 函数功能：导出侦听日志
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cms_log_listen_output_Click(object sender, EventArgs e)
        {
            try
            {
                SaveFileDialog my_savefile_dlg = new SaveFileDialog();
                my_savefile_dlg.Filter = "文本文件(*.txt)|*.txt|所有文件(*.*)|*.*";

                if (my_savefile_dlg.ShowDialog() == DialogResult.OK)
                {
                    listview_saveas_txt(my_savefile_dlg.FileName, listview_log_listen);

                    MessageBox.Show("侦听日志已经保存到指定文件。 "
                    , "提示", MessageBoxButtons.OK);
                }              
            }
            catch (Exception)
            {
                //MessageBox.Show("错误：" + ee.ToString(), "提示", MessageBoxButtons.OK);
                //错误显示
                error_message_show();
                //退出程序
                Environment.Exit(0);
            }
            finally
            {
            }
        }

        #endregion


        #region 导出输出日志
        /// <summary>
        /// 函数功能：导出输出日志
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cms_log_output_output_Click(object sender, EventArgs e)
        {
            try
            {
                SaveFileDialog my_savefile_dlg = new SaveFileDialog();
                my_savefile_dlg.Filter = "文本文件(*.txt)|*.txt|所有文件(*.*)|*.*";

                if (my_savefile_dlg.ShowDialog() == DialogResult.OK)
                {
                    listview_saveas_txt(my_savefile_dlg.FileName, listview_log_output);

                    MessageBox.Show("输出日志已经保存到指定文件。 "
                   , "提示", MessageBoxButtons.OK);
                }          
            }
            catch (Exception)
            {
                //MessageBox.Show("错误：" + ee.ToString(), "提示", MessageBoxButtons.OK);
                //错误显示
                error_message_show();
                //退出程序
                Environment.Exit(0);
            }
            finally
            {
            }
        }

        #endregion


        #endregion


        #region 函数



        #region 初始化相关



        #region 界面初始化(frm_main_init方法)
        /// <summary>
        /// 函数功能：界面的初始化
        /// </summary>
        private void frm_main_init()
        {
            //侦听端口
            //端口显示标签
            lbl_listening_port_name.Text = 端口侦听控制系统.Properties.Settings.Default.grp_listening_comunication_config_port_name;
            //触发位置1

            
            //输出类型
            tab_control_output_1.SelectedIndex = 端口侦听控制系统.Properties.Settings.Default.grp_output_1_type;
            //端口
            //端口显示标签
            lbl_output_1_sp_port_name.Text =  端口侦听控制系统.Properties.Settings.Default.grp_output_1_port_comunication_config_port_name;          
            //端口写入内容
            txtbox_output_1_sp_write_data.Text = 端口侦听控制系统.Properties.Settings.Default.grp_output_1_port_write_data;
            //按键
            //消息发送内容1
            txtbox_output_1_message_data_1.Text = 端口侦听控制系统.Properties.Settings.Default.grp_output_1_message_data_1 ;
            //消息发送内容2
            txtbox_output_1_message_data_2.Text = 端口侦听控制系统.Properties.Settings.Default.grp_output_1_message_data_2;
            //消息
            //消息内容1
            txtbox_output_1_window_data_1.Text = 端口侦听控制系统.Properties.Settings.Default.grp_output_1_window_data_1;
            //消息内容2
            txtbox_output_1_window_data_2.Text = 端口侦听控制系统.Properties.Settings.Default.grp_output_1_window_data_2;


            //触发位置2

            //输出类型
            tab_control_output_2.SelectedIndex = 端口侦听控制系统.Properties.Settings.Default.grp_output_2_type;
            //端口
            //端口显示标签
            lbl_output_2_sp_port_name.Text = 端口侦听控制系统.Properties.Settings.Default.grp_output_2_port_comunication_config_port_name;
            //端口写入内容
            txtbox_output_2_sp_write_data.Text = 端口侦听控制系统.Properties.Settings.Default.grp_output_2_port_write_data;
            //按键
            //消息发送内容1
            txtbox_output_2_message_data_1.Text = 端口侦听控制系统.Properties.Settings.Default.grp_output_2_message_data_1;
            //消息发送内容2
            txtbox_output_2_message_data_2.Text = 端口侦听控制系统.Properties.Settings.Default.grp_output_2_message_data_2;
            //消息
            //消息内容1
            txtbox_output_2_window_data_1.Text = 端口侦听控制系统.Properties.Settings.Default.grp_output_2_window_data_1;
            //消息内容2
            txtbox_output_2_window_data_2.Text = 端口侦听控制系统.Properties.Settings.Default.grp_output_2_window_data_2;


            //触发位置3

            //输出类型
            tab_control_output_3.SelectedIndex = 端口侦听控制系统.Properties.Settings.Default.grp_output_3_type;
            //端口
            //端口显示标签
            lbl_output_3_sp_port_name.Text = 端口侦听控制系统.Properties.Settings.Default.grp_output_3_port_comunication_config_port_name;
            //端口写入内容
            txtbox_output_3_sp_write_data.Text = 端口侦听控制系统.Properties.Settings.Default.grp_output_3_port_write_data;
            //按键
            //消息发送内容1
            txtbox_output_3_message_data_1.Text = 端口侦听控制系统.Properties.Settings.Default.grp_output_3_message_data_1;
            //消息发送内容2
            txtbox_output_3_message_data_2.Text = 端口侦听控制系统.Properties.Settings.Default.grp_output_3_message_data_2;
            //消息
            //消息内容1
            txtbox_output_3_window_data_1.Text = 端口侦听控制系统.Properties.Settings.Default.grp_output_3_window_data_1;
            //消息内容2
            txtbox_output_3_window_data_2.Text = 端口侦听控制系统.Properties.Settings.Default.grp_output_3_window_data_2;


            //触发位置4
            //输出类型
            tab_control_output_4.SelectedIndex = 端口侦听控制系统.Properties.Settings.Default.grp_output_4_type;
            //端口
            //端口显示标签
            lbl_output_4_sp_port_name.Text = 端口侦听控制系统.Properties.Settings.Default.grp_output_4_port_comunication_config_port_name;
            //端口写入内容
            txtbox_output_4_sp_write_data.Text = 端口侦听控制系统.Properties.Settings.Default.grp_output_4_port_write_data;
            //按键
            //消息发送内容1
            txtbox_output_4_message_data_1.Text = 端口侦听控制系统.Properties.Settings.Default.grp_output_4_message_data_1;
            //消息发送内容2
            txtbox_output_4_message_data_2.Text = 端口侦听控制系统.Properties.Settings.Default.grp_output_4_message_data_2;
            //消息
            //消息内容1
            txtbox_output_4_window_data_1.Text = 端口侦听控制系统.Properties.Settings.Default.grp_output_4_window_data_1;
            //消息内容2
            txtbox_output_4_window_data_2.Text = 端口侦听控制系统.Properties.Settings.Default.grp_output_4_window_data_2;


       
            //开机自动运行

            //自动侦听
            checkbox_autorun_listening .Checked = 端口侦听控制系统.Properties.Settings.Default.autorun_listening ;
            //输出位置1 自动输出
            checkbox_autorun_output_1.Checked = 端口侦听控制系统.Properties.Settings.Default.autorun_output_1;
            //输出位置2 自动输出
            checkbox_autorun_output_2.Checked = 端口侦听控制系统.Properties.Settings.Default.autorun_output_2;
            //输出位置3 自动输出
            checkbox_autorun_output_3.Checked = 端口侦听控制系统.Properties.Settings.Default.autorun_output_3;
            //输出位置4 自动输出
            checkbox_autorun_output_4.Checked = 端口侦听控制系统.Properties.Settings.Default.autorun_output_4;

            //更新输出位置状态
            update_output_state();

          
        }
        #endregion



        #region 运行时自动开始监听和输出(frm_main_autorun 方法)
        /// <summary>
        /// 函数功能：运行时自动开始监听和输出
        /// </summary>
        private void frm_main_autorun()
        {
            //运行时自动启动选项

            //侦听
            if (端口侦听控制系统.Properties.Settings.Default.autorun_listening)
            {
                //启动侦听
                btn_listening_start.PerformClick();
            }

            //输出位置1
            if (端口侦听控制系统.Properties.Settings.Default.autorun_output_1)
            {
                btn_output_1_start_Click(null, null);
            }

            //输出位置2
            if (端口侦听控制系统.Properties.Settings.Default.autorun_output_2)
            {
                btn_output_2_start_Click(null, null);
            }

            //输出位置3
            if (端口侦听控制系统.Properties.Settings.Default.autorun_output_3)
            {
                btn_output_3_start_Click(null, null);
            }

            //输出位置4
            if (端口侦听控制系统.Properties.Settings.Default.autorun_output_4)
            {
                btn_output_4_start_Click(null, null);
            }
        }
        #endregion



        #endregion


        #region 端口侦听线程相关



        #region 启动监听线程(thread_begin_serial_port_listening 方法)
        /// <summary>
        /// 函数功能：启动监听线程
        /// </summary>
        private void thread_begin_serial_port_listening()
        {
            try
            {

            //创建线程
            thread_sp_listening = new Thread(new ThreadStart(serial_port_listening));
            thread_sp_listening.Start();

            }
            catch (Exception)
            {
                //MessageBox.Show("错误：" + ee.ToString(), "提示", MessageBoxButtons.OK);
                //错误显示
                error_message_show();
                //退出程序
                Environment.Exit(0);
            }
            finally
            {
            }

        }
        #endregion


        #region 监听串口(serial_port_listening 方法)
        /// <summary>
        /// 函数功能：监听串口
        /// </summary>
        private void serial_port_listening()
        {
            try
            {

                while (flag_in_listening)
                {
                    if (sp_listening.IsOpen)
                    {
                        // 向端口写入查询数据
                        sp_listening.Write("FF");
                        // 读取端口返回数据
                        byte[] buffer = new byte[sp_listening.BytesToRead];
                        sp_listening.Read(buffer, 0, buffer.Length);


                        // 如果有返回数据
                        if (buffer.Length > 0)
                        {
                            //且返回数据与上一状态不同
                            if (Convert.ToString(buffer[0], 2) != listening_old_state)
                            {

                                // 最后状态赋值
                                listening_old_state = Convert.ToString(buffer[0], 2);

                                //判断触发位置
                                trigger_positon_estimate(listening_old_state.Substring(4, 4));

                                //触发状态赋值
                                trigger_old_state = listening_old_state.Substring(4, 4);

                                //添加到listview_log
                                listview_log_listen_add_data(trigger_old_state);

                            }
                        }
                    }

                    //延时
                    Thread.Sleep(listening_thread_sleep);
                }

                //关闭端口
                sp_listening.Close();
                //释放资源
                sp_listening.Dispose();

            }
            catch (Exception)
            {
                //MessageBox.Show("错误：" + ee.ToString(), "提示", MessageBoxButtons.OK);
                //错误显示
                error_message_show();
                //退出程序
                Environment.Exit(0);
            }
            finally
            {
            }

        }
        #endregion



        #endregion


        #region listview_log 相关


        #region listview_log_listen 添加数据(listview_log_listen_add_data 方法)
        /// <summary>
        /// 函数功能：向listview_log中添加数据
        /// </summary>
        /// <param name="datatime"></param>
        /// <param name="state"></param>
        private void listview_log_listen_add_data(string state)
        {
            lock (mutex_log_listen)
            {
                //添加表头
                //序列
                ListViewItem my_listview_item = new ListViewItem((listview_log_listen.Items.Count + 1).ToString());
                listview_log_listen.Items.Add(my_listview_item);
                string date = DateTime.Now.ToLongDateString() + " " + DateTime.Now.ToLongTimeString() + " " + DateTime.Now.Millisecond;

                //添加表项
                //时间
                listview_log_listen.Items[listview_log_listen.Items.Count - 1].SubItems.Add(date);
                //状态
                listview_log_listen.Items[listview_log_listen.Items.Count - 1].SubItems.Add(state);

                //保证此行向用户显示
                listview_log_listen.Items[listview_log_listen.Items.Count - 1].EnsureVisible();
            }

        }
        #endregion


        #region listview_log_output 添加数据(listview_log_output_add_data 方法)
        /// <summary>
        /// 函数功能：向listview_log_output中添加数据
        /// </summary>
        /// <param name="datatime"></param>
        /// <param name="state"></param>
        private void listview_log_output_add_data(string state)
        {
            lock (mutex_log_output)
            {
                //添加表头
                //序列
                ListViewItem my_listview_item = new ListViewItem((listview_log_output.Items.Count + 1).ToString());
                listview_log_output.Items.Add(my_listview_item);

                string date = DateTime.Now.ToLongDateString() + " " + DateTime.Now.ToLongTimeString() + " " + DateTime.Now.Millisecond;

                //添加表项
                //时间
                listview_log_output.Items[listview_log_output.Items.Count - 1].SubItems.Add(date);
                //状态
                listview_log_output.Items[listview_log_output.Items.Count - 1].SubItems.Add(state);

                //保证此行向用户显示
                listview_log_output.Items[listview_log_output.Items.Count - 1].EnsureVisible();
            }
        }
        #endregion


        #endregion


        #region 触发器相关


        #region 触发位置判断(trigger_position_estimate)
        /// <summary>
        /// 函数功能：触发位置判断
        /// </summary>
        /// <param name="trigger_now_state"></param>
        private void trigger_positon_estimate(string trigger_now_state)
        {
            //判断触发位置1
            if (trigger_now_state .Substring (0,1)!=trigger_old_state .Substring (0,1))
            {               
                //设置触发标志位
                flag_output_1_trigger = true;
                
            }


            //判断触发位置2
            if (trigger_now_state.Substring(1, 1) != trigger_old_state.Substring(1, 1))
            {
                //设置触发标志位
                flag_output_2_trigger = true;

            }


            //判断触发位置3
            if (trigger_now_state.Substring(2, 1) != trigger_old_state.Substring(2, 1))
            {
                //设置触发标志位
                flag_output_3_trigger = true;

            }


            //判断触发位置4
            if (trigger_now_state.Substring(3, 1) != trigger_old_state.Substring(3, 1))
            {
                //设置触发标志位
                flag_output_4_trigger = true;

            }
        }
        #endregion


        #region 检查此次触发是否要被忽略(trigger_is_neglect)
       /// <summary>
        /// 函数功能：检查此次触发是否要被忽略
       /// </summary>
       /// <param name="neglect_config"></param>
       /// <param name="output_position_trigger_indicator"></param>
       /// <returns></returns>
        private bool trigger_is_neglect(string neglect_config , string output_position_trigger_indicator)
        {
            bool output = false ;
            
            //如果忽略字符串不为空
            if (neglect_config.Trim().Length > 0)
            {
                //检查此次触发是否要被忽略
                string[] temp = neglect_config.Split('/');
                //组
                string[] neglect_group;
                //左端值
                int neglect_left;
                //右端值
                int neglect_right;
                //当前计数值
                int value;
                int.TryParse(output_position_trigger_indicator, out value);
                for (int i = 0; i < temp .Length ; i++)
                {
                    neglect_group = temp[i].Split('-');
                    int.TryParse(neglect_group[0], out neglect_left);
                    int.TryParse(neglect_group[1], out neglect_right);
                    if (value >= neglect_left && value <= neglect_right)
                    {
                        output = true;
                        return output;
                    }
                }
            }
            return output;

        }
        #endregion


        #endregion


        #region 输出相关


        #region 输出位置1输出相关


        #region 启动输出位置1线程(begin_thread_output_position_1_process)
        /// <summary>
        /// 函数功能：启动输出位置1的线程
        /// </summary>
        private void begin_thread_output_position_1_process()
        {
            try
            {

                //创建线程
                thread_output_position_1_proess = new Thread(new ThreadStart(output_position_1_process));
                thread_output_position_1_proess.Start();

            }
            catch (Exception)
            {
                //MessageBox.Show("错误：" + ee.ToString(), "提示", MessageBoxButtons.OK);
                //错误显示
                error_message_show();
                //退出程序
                Environment.Exit(0);
            }
            finally
            {
            }
        }
        #endregion


        #region 输出位置1输出处理(output_position_1_process)
        /// <summary>
        /// 函数功能：处理输出位置1的数据
        /// </summary>
        private void output_position_1_process()
        {
            try
            {

                while (flag_output_position_1_in_output)
                {
                    //如果有触发
                    if (flag_output_1_trigger)
                    {
                        //循环内触发计数器加一
                        output_position_1_trigger_indicator++;
                        //更新显示端口1触发次数统计(循环内)                   
                        lbl_trigger_position_1_indicator.Text = output_position_1_trigger_indicator.ToString();

                        //如果是第一次触发
                        if (output_position_1_trigger_indicator == 1)
                        {
                            //定时器开始计时
                            timer_output_1.Start();

                            if (端口侦听控制系统.Properties.Settings.Default.grp_output_1_control_config_shoot_delay > 0)
                            {
                                //线程延时
                                Thread.Sleep(端口侦听控制系统.Properties.Settings.Default.grp_output_1_control_config_shoot_delay);
                            }
                        }

                        //如果此次触发未被忽略
                        if (!trigger_is_neglect(端口侦听控制系统.Properties.Settings.Default.grp_output_1_control_config_shoot_neglect, output_position_1_trigger_indicator.ToString()))
                        {
                            output_position_1_output();
                        }


                        //如果需启动额外输出定时器
                        if (output_position_1_trigger_indicator == 端口侦听控制系统.Properties.Settings.Default.grp_output_1_control_config_additional_number)
                        {
                            //启动额外输出定时器
                            timer_output_addition_1.Start();
                        }

                        //重置触发标志位
                        flag_output_1_trigger = false;
                    }
                    //线程暂停
                    Thread.Sleep(output_thread_sleep);
                }
                //释放资源

                //关闭端口
                sp_output_position_1.Close();
                //释放端口
                sp_output_position_1.Dispose();
                //关闭计时器
                timer_output_1.Stop();
                //释放计时器
                timer_output_1.Dispose();
                //关闭额外输出计时器
                timer_output_addition_1.Stop();
                //释放计时器
                timer_output_addition_1.Dispose();

            }
            catch (Exception)
            {
                //MessageBox.Show("错误：" + ee.ToString(), "提示", MessageBoxButtons.OK);
                //错误显示
                error_message_show();
                //退出程序
                Environment.Exit(0);
            }
            finally
            {
            }
        }
        #endregion


        #region 输出位置1输出函数(output_position_1_output)
        /// <summary>
        /// 函数功能：进行输出
        /// </summary>
        private void output_position_1_output()
        {
            try
            {

                //输出次数加一
                output_position_1_output_indicator++;
                //显示输出次数
                lbl_output_position_1_indicator.Text = output_position_1_output_indicator.ToString();

                //判断输出方式
                //如果是端口输出
                if (端口侦听控制系统.Properties.Settings.Default.grp_output_1_type.ToString() == "0")
                {
                    //如果要写入的数据不为空
                    if (端口侦听控制系统.Properties.Settings.Default.grp_output_1_port_write_data.Trim().Length > 0)
                    {

                        //如果输出端口未打开，打开输出端口
                        if (!sp_output_position_1.IsOpen)
                        {
                            sp_output_position_1.Open();
                        }

                        //写入数据                            
                        //取得写入的数据
                        string[] sp_data = 端口侦听控制系统.Properties.Settings.Default.grp_output_1_port_write_data.Split(' ');
                        byte[] byte_data = new byte[sp_data.Length];
                        int out_result = 0;
                        for (int i = 0; i < sp_data.Length; i++)
                        {
                            int.TryParse(sp_data[i], out out_result);

                            byte_data[i] = (byte)out_result;
                        }
                        sp_output_position_1.Write(byte_data, 0, byte_data.Length);
                    }
                }
                //如果是按键输出
                else if (端口侦听控制系统.Properties.Settings.Default.grp_output_1_type.ToString() == "1")
                {
                    //发送消息
                    winapi_get_and_click(端口侦听控制系统.Properties.Settings.Default.grp_output_1_message_data_1, 端口侦听控制系统.Properties.Settings.Default.grp_output_1_message_data_2);
                }
                //如果是消息输出
                else if (端口侦听控制系统.Properties.Settings.Default.grp_output_1_type.ToString() == "2")
                {
                    //发送消息
                    winapi_send_window_message(端口侦听控制系统.Properties.Settings.Default.grp_output_1_window_data_1, 端口侦听控制系统.Properties.Settings.Default.grp_output_1_window_data_2);
                }

                listview_log_output_add_data("输出端口1输出");
            }
            catch (Exception)
            {
                //MessageBox.Show("错误：" + ee.ToString(), "提示", MessageBoxButtons.OK);
                //错误显示
                error_message_show();
                //退出程序
                Environment.Exit(0);
            }
            finally
            {
            }
        }
        #endregion


        #endregion


        #region 输出位置2输出相关


        #region 启动输出位置2线程(begin_thread_output_position_2_process)
        /// <summary>
        /// 函数功能：启动输出位置2的线程
        /// </summary>
        private void begin_thread_output_position_2_process()
        {
            try
            {
                //创建线程
                thread_output_position_2_proess = new Thread(new ThreadStart(output_position_2_process));
                thread_output_position_2_proess.Start();
            }
            catch (Exception)
            {
                //MessageBox.Show("错误：" + ee.ToString(), "提示", MessageBoxButtons.OK);
                //错误显示
                error_message_show();
                //退出程序
                Environment.Exit(0);
            }
            finally
            {
            }
        }
        #endregion


        #region 输出位置2输出处理(output_position_2_process)
        /// <summary>
        /// 函数功能：处理输出位置2的数据
        /// </summary>
        private void output_position_2_process()
        {
            try
            {

                while (flag_output_position_2_in_output)
                {
                    //如果有触发
                    if (flag_output_2_trigger)
                    {
                        //循环内触发计数器加一
                        output_position_2_trigger_indicator++;
                        //更新显示端口2触发次数统计(循环内)                   
                        lbl_trigger_position_2_indicator.Text = output_position_2_trigger_indicator.ToString();

                        //如果是第一次触发
                        if (output_position_2_trigger_indicator == 1)
                        {
                            //定时器开始计时
                            timer_output_2.Start();

                            if (端口侦听控制系统.Properties.Settings.Default.grp_output_2_control_config_shoot_delay > 0)
                            {
                                //线程延时
                                Thread.Sleep(端口侦听控制系统.Properties.Settings.Default.grp_output_2_control_config_shoot_delay);
                            }
                        }

                        //如果此次触发未被忽略
                        if (!trigger_is_neglect(端口侦听控制系统.Properties.Settings.Default.grp_output_2_control_config_shoot_neglect, output_position_2_trigger_indicator.ToString()))
                        {
                            output_position_2_output();
                        }

                        //如果需启动额外输出定时器
                        if (output_position_2_trigger_indicator == 端口侦听控制系统.Properties.Settings.Default.grp_output_2_control_config_additional_number)
                        {
                            //启动额外输出定时器
                            timer_output_addition_2.Start();
                        }

                        //重置触发标志位
                        flag_output_2_trigger = false;
                    }

                    //线程暂停
                    Thread.Sleep(output_thread_sleep);
                }
                //释放资源

                //关闭端口
                sp_output_position_2.Close();
                //释放端口
                sp_output_position_2.Dispose();
                //关闭计时器
                timer_output_2.Stop();
                //关闭计时器
                timer_output_2.Dispose();
                //关闭额外输出计时器
                timer_output_addition_2.Stop();
                //释放计时器
                timer_output_addition_2.Dispose();

            }
            catch (Exception)
            {
                //MessageBox.Show("错误：" + ee.ToString(), "提示", MessageBoxButtons.OK);
                //错误显示
                error_message_show();
                //退出程序
                Environment.Exit(0);
            }
            finally
            {
            }
        }
        #endregion


        #region 输出位置2输出函数(output_position_2_output)
        /// <summary>
        /// 函数功能：进行输出
        /// </summary>
        private void output_position_2_output()
        {
            try
            {

                //输出次数加一
                output_position_2_output_indicator++;
                //显示输出次数
                lbl_output_position_2_indicator.Text = output_position_2_output_indicator.ToString();

                //判断输出方式
                //如果是端口输出
                if (端口侦听控制系统.Properties.Settings.Default.grp_output_2_type.ToString() == "0")
                {
                    //如果要写入的数据不为空
                    if (端口侦听控制系统.Properties.Settings.Default.grp_output_2_port_write_data.Trim().Length > 0)
                    {
                        //如果输出端口未打开，打开输出端口
                        if (!sp_output_position_2.IsOpen)
                        {
                            sp_output_position_2.Open();
                        }

                        //写入数据
                        //取得写入的数据
                        string[] sp_data = 端口侦听控制系统.Properties.Settings.Default.grp_output_2_port_write_data.Split(' ');
                        byte[] byte_data = new byte[sp_data.Length];
                        int out_result = 0;
                        for (int i = 0; i < sp_data.Length; i++)
                        {
                            int.TryParse(sp_data[i], out out_result);

                            byte_data[i] = (byte)out_result;
                        }
                        sp_output_position_2.Write(byte_data, 0, byte_data.Length);
                    }
                }
                //如果是按键输出
                else if (端口侦听控制系统.Properties.Settings.Default.grp_output_2_type.ToString() == "1")
                {
                    //发送消息
                    winapi_get_and_click(端口侦听控制系统.Properties.Settings.Default.grp_output_2_message_data_1, 端口侦听控制系统.Properties.Settings.Default.grp_output_2_message_data_2);
                }
                //如果是消息输出
                else if (端口侦听控制系统.Properties.Settings.Default.grp_output_2_type.ToString() == "2")
                {
                    //发送消息
                    winapi_send_window_message(端口侦听控制系统.Properties.Settings.Default.grp_output_2_window_data_1, 端口侦听控制系统.Properties.Settings.Default.grp_output_2_window_data_2);
                }
                //输出日志
                listview_log_output_add_data("输出端口2输出");

            }
            catch (Exception)
            {
                //MessageBox.Show("错误：" + ee.ToString(), "提示", MessageBoxButtons.OK);
                //错误显示
                error_message_show();
                //退出程序
                Environment.Exit(0);
            }
            finally
            {
            }
        }
        #endregion


        #endregion


        #region 输出位置3输出相关


        #region 启动输出位置3线程(begin_thread_output_position_3_process)
        /// <summary>
        /// 函数功能：启动输出位置3的线程
        /// </summary>
        private void begin_thread_output_position_3_process()
        {
            try
            {

                //创建线程
                thread_output_position_3_proess = new Thread(new ThreadStart(output_position_3_process));
                thread_output_position_3_proess.Start();

            }
            catch (Exception)
            {
                //MessageBox.Show("错误：" + ee.ToString(), "提示", MessageBoxButtons.OK);
                //错误显示
                error_message_show();
                //退出程序
                Environment.Exit(0);
            }
            finally
            {
            }
        }
        #endregion


        #region 输出位置3输出处理(output_position_3_process)
        /// <summary>
        /// 函数功能：处理输出位置3的数据
        /// </summary>
        private void output_position_3_process()
        {
            try
            {

                while (flag_output_position_3_in_output)
                {
                    //如果有触发
                    if (flag_output_3_trigger)
                    {
                        //循环内触发计数器加一
                        output_position_3_trigger_indicator++;
                        //更新显示端口3触发次数统计(循环内)                   
                        lbl_trigger_position_3_indicator.Text = output_position_3_trigger_indicator.ToString();


                        //如果是第一次触发
                        if (output_position_3_trigger_indicator == 1)
                        {
                            //定时器开始计时
                            timer_output_3.Start();

                            if (端口侦听控制系统.Properties.Settings.Default.grp_output_3_control_config_shoot_delay > 0)
                            {
                                //线程延时
                                Thread.Sleep(端口侦听控制系统.Properties.Settings.Default.grp_output_3_control_config_shoot_delay);
                            }
                        }

                        //如果此次触发未被忽略
                        if (!trigger_is_neglect(端口侦听控制系统.Properties.Settings.Default.grp_output_3_control_config_shoot_neglect, output_position_3_trigger_indicator.ToString()))
                        {
                            output_position_3_output();
                        }

                        //如果需启动额外输出定时器
                        if (output_position_3_trigger_indicator == 端口侦听控制系统.Properties.Settings.Default.grp_output_3_control_config_additional_number)
                        {
                            //启动额外输出定时器
                            timer_output_addition_3.Start();
                        }

                        //重置触发标志位
                        flag_output_3_trigger = false;
                    }

                    //线程暂停
                    Thread.Sleep(output_thread_sleep);
                }
                //释放资源

                //关闭端口
                sp_output_position_3.Close();
                //释放端口
                sp_output_position_3.Dispose();
                //关闭计时器
                timer_output_3.Stop();
                //关闭计时器
                timer_output_3.Dispose();
                //关闭额外输出计时器
                timer_output_addition_3.Stop();
                //释放计时器
                timer_output_addition_3.Dispose();

            }
            catch (Exception)
            {
                //MessageBox.Show("错误：" + ee.ToString(), "提示", MessageBoxButtons.OK);
                //错误显示
                error_message_show();
                //退出程序
                Environment.Exit(0);
            }
            finally
            {
            }
        }
        #endregion


        #region 输出位置3输出函数(output_position_3_output)
        /// <summary>
        /// 函数功能：进行输出
        /// </summary>
        private void output_position_3_output()
        {
            try
            {

                //输出次数加一
                output_position_3_output_indicator++;
                //显示输出次数
                lbl_output_position_3_indicator.Text = output_position_3_output_indicator.ToString();

                //判断输出方式
                //如果是端口输出
                if (端口侦听控制系统.Properties.Settings.Default.grp_output_3_type.ToString() == "0")
                {
                    //如果要写入的数据不为空
                    if (端口侦听控制系统.Properties.Settings.Default.grp_output_3_port_write_data.Trim().Length > 0)
                    {
                        //如果输出端口未打开，打开输出端口
                        if (!sp_output_position_3.IsOpen)
                        {
                            sp_output_position_3.Open();
                        }

                        //写入数据

                        //取得写入的数据
                        string[] sp_data = 端口侦听控制系统.Properties.Settings.Default.grp_output_3_port_write_data.Split(' ');
                        byte[] byte_data = new byte[sp_data.Length];
                        int out_result = 0;
                        for (int i = 0; i < sp_data.Length; i++)
                        {
                            int.TryParse(sp_data[i], out out_result);

                            byte_data[i] = (byte)out_result;
                        }
                        sp_output_position_3.Write(byte_data, 0, byte_data.Length);
                    }
                }
                //如果是按键输出
                else if (端口侦听控制系统.Properties.Settings.Default.grp_output_3_type.ToString() == "1")
                {
                    //发送消息
                    winapi_get_and_click(端口侦听控制系统.Properties.Settings.Default.grp_output_3_message_data_1, 端口侦听控制系统.Properties.Settings.Default.grp_output_3_message_data_2);
                }
                //如果是消息输出
                else if (端口侦听控制系统.Properties.Settings.Default.grp_output_3_type.ToString() == "2")
                {
                    //发送消息
                    winapi_send_window_message(端口侦听控制系统.Properties.Settings.Default.grp_output_3_window_data_1, 端口侦听控制系统.Properties.Settings.Default.grp_output_3_window_data_2);
                }

                //输出日志
                listview_log_output_add_data("输出端口3输出");
            }
            catch (Exception)
            {
                //MessageBox.Show("错误：" + ee.ToString(), "提示", MessageBoxButtons.OK);
                //错误显示
                error_message_show();
                //退出程序
                Environment.Exit(0);
            }
            finally
            {
            }
        }
        #endregion


        #endregion


        #region 输出位置4输出相关


        #region 启动输出位置4线程(begin_thread_output_position_4_process)
        /// <summary>
        /// 函数功能：启动输出位置4的线程
        /// </summary>
        private void begin_thread_output_position_4_process()
        {
            try
            {

                //创建线程
                thread_output_position_4_proess = new Thread(new ThreadStart(output_position_4_process));
                thread_output_position_4_proess.Start();

            }
            catch (Exception)
            {
                //MessageBox.Show("错误：" + ee.ToString(), "提示", MessageBoxButtons.OK);
                //错误显示
                error_message_show();
                //退出程序
                Environment.Exit(0);
            }
            finally
            {
            }
        }
        #endregion


        #region 输出位置4输出处理(output_position_4_process)
        /// <summary>
        /// 函数功能：处理输出位置4的数据
        /// </summary>
        private void output_position_4_process()
        {
            try
            {

                while (flag_output_position_4_in_output)
                {
                    //如果有触发
                    if (flag_output_4_trigger)
                    {
                        //循环内触发计数器加一
                        output_position_4_trigger_indicator++;
                        //更新显示端口4触发次数统计(循环内)                   
                        lbl_trigger_position_4_indicator.Text = output_position_4_trigger_indicator.ToString();


                        //如果是第一次触发
                        if (output_position_4_trigger_indicator == 1)
                        {
                            //定时器开始计时
                            timer_output_4.Start();

                            if (端口侦听控制系统.Properties.Settings.Default.grp_output_4_control_config_shoot_delay > 0)
                            {
                                //线程延时
                                Thread.Sleep(端口侦听控制系统.Properties.Settings.Default.grp_output_4_control_config_shoot_delay);
                            }
                        }

                        //如果此次触发未被忽略
                        if (!trigger_is_neglect(端口侦听控制系统.Properties.Settings.Default.grp_output_4_control_config_shoot_neglect, output_position_4_trigger_indicator.ToString()))
                        {
                            output_position_4_output();
                        }

                        //如果需启动额外输出定时器
                        if (output_position_4_trigger_indicator == 端口侦听控制系统.Properties.Settings.Default.grp_output_4_control_config_additional_number)
                        {
                            //启动额外输出定时器
                            timer_output_addition_4.Start();
                        }

                        //重置触发标志位
                        flag_output_4_trigger = false;
                    }

                    //线程暂停
                    Thread.Sleep(output_thread_sleep);
                }
                //释放资源

                //关闭端口
                sp_output_position_4.Close();
                //释放端口
                sp_output_position_4.Dispose();
                //关闭计时器
                timer_output_4.Stop();
                //关闭计时器
                timer_output_4.Dispose();
                //关闭额外输出计时器
                timer_output_addition_4.Stop();
                //释放计时器
                timer_output_addition_4.Dispose();

            }
            catch (Exception)
            {
                //MessageBox.Show("错误：" + ee.ToString(), "提示", MessageBoxButtons.OK);
                //错误显示
                error_message_show();
                //退出程序
                Environment.Exit(0);
            }
            finally
            {
            }
        }
        #endregion


        #region 输出位置4输出函数(output_position_4_output)
        /// <summary>
        /// 函数功能：进行输出
        /// </summary>
        private void output_position_4_output()
        {
            try
            {

                //输出次数加一
                output_position_4_output_indicator++;
                //显示输出次数
                lbl_output_position_4_indicator.Text = output_position_4_output_indicator.ToString();

                //判断输出方式
                //如果是端口输出
                if (端口侦听控制系统.Properties.Settings.Default.grp_output_4_type.ToString() == "0")
                {
                    //如果要写入的数据不为空
                    if (端口侦听控制系统.Properties.Settings.Default.grp_output_4_port_write_data.Trim().Length > 0)
                    {
                        //如果输出端口未打开，打开输出端口
                        if (!sp_output_position_4.IsOpen)
                        {
                            sp_output_position_4.Open();
                        }

                        //写入数据

                        //取得写入的数据
                        string[] sp_data = 端口侦听控制系统.Properties.Settings.Default.grp_output_4_port_write_data.Split(' ');
                        byte[] byte_data = new byte[sp_data.Length];
                        int out_result = 0;
                        for (int i = 0; i < sp_data.Length; i++)
                        {
                            int.TryParse(sp_data[i], out out_result);

                            byte_data[i] = (byte)out_result;
                        }
                        sp_output_position_4.Write(byte_data, 0, byte_data.Length);
                    }
                }
                //如果是按键输出
                else if (端口侦听控制系统.Properties.Settings.Default.grp_output_4_type.ToString() == "1")
                {
                    //发送消息
                    winapi_get_and_click(端口侦听控制系统.Properties.Settings.Default.grp_output_4_message_data_1, 端口侦听控制系统.Properties.Settings.Default.grp_output_4_message_data_2);
                }
                //如果是消息输出
                else if (端口侦听控制系统.Properties.Settings.Default.grp_output_4_type.ToString() == "2")
                {
                    //发送消息
                    winapi_send_window_message(端口侦听控制系统.Properties.Settings.Default.grp_output_4_window_data_1, 端口侦听控制系统.Properties.Settings.Default.grp_output_4_window_data_2);
                }

                //输出日志
                listview_log_output_add_data("输出端口4输出");
            }
            catch (Exception)
            {
                //MessageBox.Show("错误：" + ee.ToString(), "提示", MessageBoxButtons.OK);
                //错误显示
                error_message_show();
                //退出程序
                Environment.Exit(0);
            }
            finally
            {
            }
        }
        #endregion


        #endregion


        #region 更新显示输出位置状态(update_output_state 函数)
        /// <summary>
        /// 函数功能：更新显示输出位置状态
        /// </summary>
        public  void update_output_state()
        {
            //更新输出位置状态

            //输出位置一的状态
            string output_position_1_state = "";
            //输出方式
            if (端口侦听控制系统.Properties.Settings.Default.grp_output_1_type == 0)
            {
                output_position_1_state =output_position_1_state + "端口"+ 端口侦听控制系统 .Properties .Settings .Default .grp_output_1_port_comunication_config_port_name ;
            }
            else if(端口侦听控制系统.Properties.Settings.Default.grp_output_1_type == 1)
            {
                 output_position_1_state =output_position_1_state + "按键";
            }
            else if (端口侦听控制系统.Properties.Settings.Default.grp_output_1_type == 2)
            {
                output_position_1_state = output_position_1_state + "消息";
            }
            //输出状态
            if (flag_output_position_1_in_output)
            {
                 output_position_1_state =output_position_1_state + "/"+"开启";
            }
            else 
            {
                output_position_1_state = output_position_1_state + "/" + "关闭";
            }
            //自动输出
            if (端口侦听控制系统.Properties.Settings.Default.autorun_output_1)
            {
                output_position_1_state = output_position_1_state + "/" + "自动输出";
            }

            //控制设置
            output_position_1_state = output_position_1_state + " (";
            //循环
            output_position_1_state = output_position_1_state + "循环:" + 端口侦听控制系统.Properties.Settings.Default.grp_output_1_control_config_series_shoot_timer.ToString() + "ms ";
            //延时
            output_position_1_state = output_position_1_state + "延时:" + 端口侦听控制系统.Properties.Settings.Default.grp_output_1_control_config_shoot_delay .ToString() + "ms ";
            //忽略
            output_position_1_state = output_position_1_state + "忽略:" + 端口侦听控制系统.Properties.Settings.Default.grp_output_1_control_config_shoot_neglect .ToString() + " ";
      
            //追加输出
            //追加次数序号
            output_position_1_state = output_position_1_state + "序号:" + 端口侦听控制系统.Properties .Settings .Default .grp_output_1_control_config_additional_number .ToString ()+ " " ;
            //追加次数数量
            output_position_1_state = output_position_1_state + "追加:" +  端口侦听控制系统.Properties.Settings.Default.grp_output_1_control_config_additional_amount.ToString() + " ";
            //追加输出间隔
            output_position_1_state = output_position_1_state + "间隔:" + 端口侦听控制系统.Properties.Settings.Default.grp_output_1_control_config_additional_interval.ToString() + "ms";
            output_position_1_state = output_position_1_state + " )";

            //更新显示状态
            lbl_output_position_1_state.Text = output_position_1_state;
            //设置标签颜色
            if (flag_output_position_1_in_output)
            {
                lbl_output_position_1_state.ForeColor = Color.Blue;
            }
            else
            {
                lbl_output_position_1_state.ForeColor = Color.Black;
            }


            //输出位置二的状态
            string output_position_2_state = "";
            //输出方式
            if (端口侦听控制系统.Properties.Settings.Default.grp_output_2_type == 0)
            {
                output_position_2_state = output_position_2_state + "端口" + 端口侦听控制系统.Properties.Settings.Default.grp_output_2_port_comunication_config_port_name; 
            }
            else if (端口侦听控制系统.Properties.Settings.Default.grp_output_2_type == 1)
            {
                output_position_2_state = output_position_2_state + "按键";
            }
            else if (端口侦听控制系统.Properties.Settings.Default.grp_output_2_type == 2)
            {
                output_position_2_state = output_position_2_state + "消息";
            }
            //输出状态
            if (flag_output_position_2_in_output)
            {
                output_position_2_state = output_position_2_state + "/" + "开启";
            }
            else
            {
                output_position_2_state = output_position_2_state + "/" + "关闭";
            }
            //自动输出
            if (端口侦听控制系统.Properties.Settings.Default.autorun_output_2)
            {
                output_position_2_state = output_position_2_state + "/" + "自动输出";
            }

            //控制设置
            output_position_2_state = output_position_2_state + " (";
            //循环
            output_position_2_state = output_position_2_state + "循环:" + 端口侦听控制系统.Properties.Settings.Default.grp_output_2_control_config_series_shoot_timer.ToString() + "ms ";
            //延时
            output_position_2_state = output_position_2_state + "延时:" + 端口侦听控制系统.Properties.Settings.Default.grp_output_2_control_config_shoot_delay.ToString() + "ms ";
            //忽略
            output_position_2_state = output_position_2_state + "忽略:" + 端口侦听控制系统.Properties.Settings.Default.grp_output_2_control_config_shoot_neglect.ToString() + " ";

            //追加输出
            //追加次数序号
            output_position_2_state = output_position_2_state + "序号:" + 端口侦听控制系统.Properties.Settings.Default.grp_output_2_control_config_additional_number.ToString() + " ";
            //追加次数数量
            output_position_2_state = output_position_2_state + "追加:" + 端口侦听控制系统.Properties.Settings.Default.grp_output_2_control_config_additional_amount.ToString() + " ";
            //追加输出间隔
            output_position_2_state = output_position_2_state + "间隔:" + 端口侦听控制系统.Properties.Settings.Default.grp_output_2_control_config_additional_interval.ToString() + "ms";
            output_position_2_state = output_position_2_state + " )";

            //更新显示状态
            lbl_output_position_2_state.Text = output_position_2_state;
            //设置标签颜色
            if (flag_output_position_2_in_output)
            {
                lbl_output_position_2_state.ForeColor = Color.Blue;
            }
            else
            {
                lbl_output_position_2_state.ForeColor = Color.Black;
            }


            //输出位置三的状态
            string output_position_3_state = "";
            //输出方式
            if (端口侦听控制系统.Properties.Settings.Default.grp_output_3_type == 0)
            {
                output_position_3_state = output_position_3_state + "端口" + 端口侦听控制系统.Properties.Settings.Default.grp_output_3_port_comunication_config_port_name; 
            }
            else if (端口侦听控制系统.Properties.Settings.Default.grp_output_3_type == 1)
            {
                output_position_3_state = output_position_3_state + "按键";
            }
            else if (端口侦听控制系统.Properties.Settings.Default.grp_output_3_type == 2)
            {
                output_position_3_state = output_position_3_state + "消息";
            }
            //输出状态
            if (flag_output_position_3_in_output)
            {
                output_position_3_state = output_position_3_state + "/" + "开启";
            }
            else
            {
                output_position_3_state = output_position_3_state + "/" + "关闭";
            }
            //自动输出
            if (端口侦听控制系统.Properties.Settings.Default.autorun_output_3)
            {
                output_position_3_state = output_position_3_state + "/" + "自动输出";
            }

            //控制设置
            output_position_3_state = output_position_3_state + " (";
            //循环
            output_position_3_state = output_position_3_state + "循环:" + 端口侦听控制系统.Properties.Settings.Default.grp_output_3_control_config_series_shoot_timer.ToString() + "ms ";
            //延时
            output_position_3_state = output_position_3_state + "延时:" + 端口侦听控制系统.Properties.Settings.Default.grp_output_3_control_config_shoot_delay.ToString() + "ms ";
            //忽略
            output_position_3_state = output_position_3_state + "忽略:" + 端口侦听控制系统.Properties.Settings.Default.grp_output_3_control_config_shoot_neglect.ToString() + " ";

            //追加输出
            //追加次数序号
            output_position_3_state = output_position_3_state + "序号:" + 端口侦听控制系统.Properties.Settings.Default.grp_output_3_control_config_additional_number.ToString() + " ";
            //追加次数数量
            output_position_3_state = output_position_3_state + "追加:" + 端口侦听控制系统.Properties.Settings.Default.grp_output_3_control_config_additional_amount.ToString() + " ";
            //追加输出间隔
            output_position_3_state = output_position_3_state + "间隔:" + 端口侦听控制系统.Properties.Settings.Default.grp_output_3_control_config_additional_interval.ToString() + "ms";
            output_position_3_state = output_position_3_state + " )";

            //更新显示状态
            lbl_output_position_3_state.Text = output_position_3_state;
            //设置标签颜色
            if (flag_output_position_3_in_output)
            {
                lbl_output_position_3_state.ForeColor = Color.Blue;
            }
            else
            {
                lbl_output_position_3_state.ForeColor = Color.Black;
            }


            //输出位置四的状态
            string output_position_4_state = "";
            //输出方式
            if (端口侦听控制系统.Properties.Settings.Default.grp_output_4_type == 0)
            {
                output_position_4_state = output_position_4_state + "端口" + 端口侦听控制系统.Properties.Settings.Default.grp_output_4_port_comunication_config_port_name; ;
            }
            else if (端口侦听控制系统.Properties.Settings.Default.grp_output_4_type == 1)
            {
                output_position_4_state = output_position_4_state + "按键";
            }
            else if (端口侦听控制系统.Properties.Settings.Default.grp_output_4_type == 2)
            {
                output_position_4_state = output_position_4_state + "消息";
            }
            //输出状态
            if (flag_output_position_4_in_output)
            {
                output_position_4_state = output_position_4_state + "/" + "开启";
            }
            else
            {
                output_position_4_state = output_position_4_state + "/" + "关闭";
            }
            //自动输出
            if (端口侦听控制系统.Properties.Settings.Default.autorun_output_4)
            {
                output_position_4_state = output_position_4_state + "/" + "自动输出";
            }

            //控制设置
            output_position_4_state = output_position_4_state + " (";
            //循环
            output_position_4_state = output_position_4_state + "循环:" + 端口侦听控制系统.Properties.Settings.Default.grp_output_4_control_config_series_shoot_timer.ToString() + "ms ";
            //延时
            output_position_4_state = output_position_4_state + "延时:" + 端口侦听控制系统.Properties.Settings.Default.grp_output_4_control_config_shoot_delay.ToString() + "ms ";
            //忽略
            output_position_4_state = output_position_4_state + "忽略:" + 端口侦听控制系统.Properties.Settings.Default.grp_output_4_control_config_shoot_neglect.ToString() + " ";

            //追加输出
            //追加次数序号
            output_position_4_state = output_position_4_state + "序号:" + 端口侦听控制系统.Properties.Settings.Default.grp_output_4_control_config_additional_number.ToString() + " ";
            //追加次数数量
            output_position_4_state = output_position_4_state + "追加:" + 端口侦听控制系统.Properties.Settings.Default.grp_output_4_control_config_additional_amount.ToString() + " ";
            //追加输出间隔
            output_position_4_state = output_position_4_state + "间隔:" + 端口侦听控制系统.Properties.Settings.Default.grp_output_4_control_config_additional_interval.ToString() + "ms";
            output_position_4_state = output_position_4_state + " )";

            //更新显示状态
            lbl_output_position_4_state.Text = output_position_4_state;
            //设置标签颜色
            if (flag_output_position_4_in_output)
            {
                lbl_output_position_4_state.ForeColor = Color.Blue;
            }
            else
            {
                lbl_output_position_4_state.ForeColor = Color.Black;
            }
          
        }
        #endregion


        #endregion


        #region 其他函数


        #region 向特定的程序窗体发送消息(winapi_get_and_click)
        /// <summary>
        /// 函数功能：向特点的程序窗体发送消息
        /// </summary>
        /// <param name="ex_window_name"></param>
        /// <param name="function_window_name"></param>
        private void winapi_get_and_click(string ex_window_name, string function_window_name)
        {
            //设置发送消息时的窗体名称
            message_function_window_name = function_window_name;
            //查找主窗体的句柄
            IntPtr hwnd_window = FindWindow(null, ex_window_name);
            //如果找到主窗体句柄
            if (hwnd_window != IntPtr.Zero)
            {
               
                //列举所有子窗体
                EnumWindowProc my_enmuWindowProc = new EnumWindowProc(callback_enmu_WindowProc);
                EnumChildWindows(hwnd_window, my_enmuWindowProc ,IntPtr .Zero);
               
            }

            //重置发送消息时的窗体名称
            message_function_window_name = "";
        }
        #endregion


        #region 向特定窗体发送消息回调部分(callback_enmu_WindowProc)
        /// <summary>
        /// 函数功能：向特定窗体发送消息回调部分
        /// </summary>
        private bool callback_enmu_WindowProc(IntPtr hWnd, IntPtr lParam)
        {
            //鼠标点击的消息，对于各种消息的数值，大家还是得去API手册
            const uint BM_CLICK = 0xF5;

            //窗体类型
            StringBuilder  class_name = new StringBuilder (256);
            //窗体名称
            StringBuilder window_text = new StringBuilder (256);
           //查找button
            GetClassName(hWnd, class_name, 256);
            if (class_name.ToString().Trim () == "Button")
            {
                //查找标题
                GetWindowText(hWnd, window_text, 256);
                if (window_text.ToString ().Trim () == message_function_window_name)
                {
                    //将主窗体设为当前活动窗口
                    SetForegroundWindow(hWnd);

                    //发送子窗体点击消息
                    SendMessage(hWnd, BM_CLICK, 0, 0);               
                }
            }
            return true;
        }
        #endregion


        #region 向特定的程序窗体发送消息(winapi_send_window_message)
        /// <summary>
        /// 函数功能：向特点的程序窗体发送消息
        /// </summary>
        /// <param name="ex_window_name"></param>
        /// <param name="function_window_name"></param>
        private void winapi_send_window_message(string ex_window_name, string message_data)
        {
            
            //查找主窗体的句柄
            IntPtr hwnd_window = FindWindow(null, ex_window_name);
            //如果找到主窗体句柄
            if (hwnd_window != IntPtr.Zero)
            {
                uint  my_message_date = 0;
                uint.TryParse(message_data, out my_message_date);

                //发送子窗体点击消息
                SendMessage(hwnd_window ,my_message_date , 0, 0);    
            }
        }
        #endregion


        #endregion


        #region 错误处理


        #region 返回错误信息
        /// <summary>
        /// 函数功能： 错误处理
        /// </summary>
        private void error_message_show()
        {
            MessageBox.Show("软件发生错误，请与提供商联系！", "提示", MessageBoxButtons.OK);
        }
        #endregion


        #endregion


        #region 日志相关


        #region 将listview 数据保存为txt（listview_saveas_txt 方法）
        /// <summary>
        /// 函数功能：将listview 数据保存为txt
        /// </summary>
        /// <param name="file_name"></param>
        /// <param name="my_list_view"></param>
        public void listview_saveas_txt(string file_name, ListView my_list_view)
        {
            FileStream myfs = new FileStream(file_name, FileMode.Create);
            StreamWriter mysw = new StreamWriter(myfs);

            //记录每列的最大字符数的数组
            int[] max_length = new int[my_list_view.Columns.Count];

            //初始化该数组
            for (int i = 0; i < my_list_view.Columns.Count; i++)
            {
                max_length[i] = System.Text.Encoding.Default.GetByteCount(my_list_view.Columns[i].Text.Trim().ToString());
            }

            //扫描取得每列最大字符数
            for (int i = 0; i < my_list_view.Columns.Count; i++)
            {
                for (int j = 0; j < my_list_view.Items.Count; j++)
                {
                    if (System.Text.Encoding.Default.GetByteCount(my_list_view.Items[j].SubItems[i].Text.Trim().ToString()) > max_length[i])
                    {
                        max_length[i] = System.Text.Encoding.Default.GetByteCount(my_list_view.Items[j].SubItems[i].Text.Trim().ToString());
                    }
                }

            }


            //保存表头
            for (int i = 0; i < my_list_view.Columns.Count; i++)
            {
                //写入内容
                mysw.Write(my_list_view.Columns[i].Text.Trim().ToString());
                //补足空格
                for (int space = 0; space < max_length[i] - System.Text.Encoding.Default.GetByteCount(my_list_view.Columns[i].Text.Trim().ToString()); space++)
                {
                    mysw.Write(" ");
                }
                //写入空格
                mysw.Write("     ");

            }
            //写入换行
            mysw.Write("\r\n");

            //保存表内容
            for (int i = 0; i < my_list_view.Items.Count; i++)
            {
                //保存一行的内容
                for (int j = 0; j < my_list_view.Columns.Count; j++)
                {
                    //写入内容
                    mysw.Write(my_list_view.Items[i].SubItems[j].Text.Trim().ToString());
                    //补足空格
                    for (int space = 0; space < max_length[j] - System.Text.Encoding.Default.GetByteCount(my_list_view.Items[i].SubItems[j].Text.Trim().ToString()); space++)
                    {
                        mysw.Write(" ");
                    }
                    //写入空格
                    mysw.Write("     ");
                }
                //写入换行
                mysw.Write("\r\n");
            }
            //释放资源
            mysw.Dispose();
            myfs.Dispose();


        }


        #endregion


        #endregion


        #endregion


        #region 事件


        #region checkbox 事件


        #region checkbox_autorun_software 勾选事件
        /// <summary>
        /// 函数功能：checkbox_autorun_software被勾选时，改变保存的属性值
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void checkbox_autorun_software_CheckedChanged(object sender, EventArgs e)
        {
            //保存属性
            端口侦听控制系统.Properties.Settings.Default.autorun_listening = checkbox_autorun_listening.Checked ;
            端口侦听控制系统.Properties.Settings.Default.Save();

        }
        #endregion


        #region checkbox_autorun_output_1 勾选事件
        /// <summary>
        /// 函数功能：heckbox_autorun_output_1被勾选时，改变保存的属性值
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void checkbox_autorun_output_1_CheckedChanged(object sender, EventArgs e)
        {
            //保存属性
            端口侦听控制系统.Properties.Settings.Default.autorun_output_1 = checkbox_autorun_output_1.Checked;
            端口侦听控制系统.Properties.Settings.Default.Save();
            //更新输出位置状态
            update_output_state();
        }
        #endregion


        #region checkbox_autorun_output_2 勾选事件
        /// <summary>
        /// 函数功能：heckbox_autorun_output_2被勾选时，改变保存的属性值
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void checkbox_autorun_output_2_CheckedChanged(object sender, EventArgs e)
        {
            //保存属性
            端口侦听控制系统.Properties.Settings.Default.autorun_output_2 = checkbox_autorun_output_2.Checked;
            端口侦听控制系统.Properties.Settings.Default.Save();
            //更新输出位置状态
            update_output_state();
        }
        #endregion


        #region checkbox_autorun_output_3 勾选事件
        /// <summary>
        /// 函数功能：heckbox_autorun_output_3被勾选时，改变保存的属性值
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void checkbox_autorun_output_3_CheckedChanged(object sender, EventArgs e)
        {
            //保存属性
            端口侦听控制系统.Properties.Settings.Default.autorun_output_3 = checkbox_autorun_output_3.Checked;
            端口侦听控制系统.Properties.Settings.Default.Save();
            //更新输出位置状态
            update_output_state();
        }
        #endregion


        #region checkbox_autorun_output_4 勾选事件
        /// <summary>
        /// 函数功能：heckbox_autorun_output_4被勾选时，改变保存的属性值
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void checkbox_autorun_output_4_CheckedChanged(object sender, EventArgs e)
        {
            //保存属性
            端口侦听控制系统.Properties.Settings.Default.autorun_output_4 = checkbox_autorun_output_4.Checked;
            端口侦听控制系统.Properties.Settings.Default.Save();
            //更新输出位置状态
            update_output_state();
        }
        #endregion


        #endregion


        #region 定时器事件


        #region 输出位置1定时器tick事件
        /// <summary>
        /// 函数功能： 输出位置1定时器tick事件
       /// </summary>
       /// <param name="send"></param>
       /// <param name="e"></param>
        private void timer_output_1_tick(object send, EventArgs e)
        {
            try
            {

                //清空循环内触发计数器
                output_position_1_trigger_indicator = 0;
                lbl_trigger_position_1_indicator.Text = output_position_1_trigger_indicator.ToString();
                //清空循环内输出计数器
                output_position_1_output_indicator = 0;
                lbl_output_position_1_indicator.Text = output_position_1_output_indicator.ToString();
                //更新显示输出循环计数器
                output_position_1_recycle_indicator++;
                lbl_output_position_1_recycle_indicator.Text = output_position_1_recycle_indicator.ToString();
                //额外计时器计数状态清理
                addtional_timer_1_amount = 0;
                //定时器停止
                timer_output_1.Stop();
                //额外计时器停止
                timer_output_addition_1.Stop();

            }
            catch (Exception ee)
            {
                MessageBox.Show("错误：" + ee.ToString(), "提示", MessageBoxButtons.OK);

                //退出程序
                Environment.Exit(0);
            }
            finally
            {
            }

        }
        #endregion


        #region 输出位置2定时器tick事件
        /// <summary>
        /// 函数功能： 输出位置1定时器tick事件
        /// </summary>
        /// <param name="send"></param>
        /// <param name="e"></param>
        private void timer_output_2_tick(object send, EventArgs e)
        {
            try
            {

                //清空循环内触发计数器
                output_position_2_trigger_indicator = 0;
                //清空触发计数器显示
                lbl_trigger_position_2_indicator.Text = output_position_2_trigger_indicator.ToString();
                //清空循环内输出计数器
                output_position_2_output_indicator = 0;
                lbl_output_position_2_indicator.Text = output_position_2_output_indicator.ToString();
                //更新显示输出循环计数器
                output_position_2_recycle_indicator++;
                lbl_output_position_2_recycle_indicator.Text = output_position_2_recycle_indicator.ToString();
                //额外计时器计数状态清理
                addtional_timer_2_amount = 0;
                //定时器停止
                timer_output_2.Stop();
                //额外计时器停止
                timer_output_addition_2.Stop();

            }
            catch (Exception ee)
            {
                MessageBox.Show("错误：" + ee.ToString(), "提示", MessageBoxButtons.OK);

                //退出程序
                Environment.Exit(0);
            }
            finally
            {
            }

        }
        #endregion


        #region 输出位置3定时器tick事件
        /// <summary>
        /// 函数功能： 输出位置1定时器tick事件
        /// </summary>
        /// <param name="send"></param>
        /// <param name="e"></param>
        private void timer_output_3_tick(object send, EventArgs e)
        {
            try
            {

                //清空循环内触发计数器
                output_position_3_trigger_indicator = 0;
                //清空触发计数器显示
                lbl_trigger_position_3_indicator.Text = output_position_3_trigger_indicator.ToString();
                //清空循环内输出计数器
                output_position_3_output_indicator = 0;
                lbl_output_position_3_indicator.Text = output_position_3_output_indicator.ToString();
                //更新显示输出循环计数器
                output_position_3_recycle_indicator++;
                lbl_output_position_3_recycle_indicator.Text = output_position_3_recycle_indicator.ToString();
                //额外计时器计数状态清理
                addtional_timer_3_amount = 0;
                //定时器停止
                timer_output_3.Stop();
                //额外计时器停止
                timer_output_addition_3.Stop();

            }
            catch (Exception ee)
            {
                MessageBox.Show("错误：" + ee.ToString(), "提示", MessageBoxButtons.OK);

                //退出程序
                Environment.Exit(0);
            }
            finally
            {
            }
        }
        #endregion


        #region 输出位置4定时器tick事件
        /// <summary>
        /// 函数功能： 输出位置1定时器tick事件
        /// </summary>
        /// <param name="send"></param>
        /// <param name="e"></param>
        private void timer_output_4_tick(object send, EventArgs e)
        {
            try
            {

                //清空循环内触发计数器
                output_position_4_trigger_indicator = 0;
                //清空触发计数器显示
                lbl_trigger_position_4_indicator.Text = output_position_4_trigger_indicator.ToString();
                //清空循环内输出计数器
                output_position_4_output_indicator = 0;
                lbl_output_position_4_indicator.Text = output_position_4_output_indicator.ToString();
                //更新显示输出循环计数器
                output_position_4_recycle_indicator++;
                lbl_output_position_4_recycle_indicator.Text = output_position_4_recycle_indicator.ToString();
                //额外计时器计数状态清理
                addtional_timer_4_amount = 0;
                //定时器停止
                timer_output_4.Stop();
                //额外计时器停止
                timer_output_addition_4.Stop();

            }
            catch (Exception ee)
            {
                MessageBox.Show("错误：" + ee.ToString(), "提示", MessageBoxButtons.OK);

                //退出程序
                Environment.Exit(0);
            }
            finally
            {
            }
        }
        #endregion


        #region 模拟定时器tick事件
        /// <summary>
        /// 函数功能：模拟定时器tick事件
        /// </summary>
        /// <param name="send"></param>
        /// <param name="e"></param>
        private void timer_simulate_tick(object send, EventArgs e)
        {
            try
            {

                //触发模拟
                if (checkbox_simlate_position_1.Checked)
                {
                    flag_output_1_trigger = true;
                    //添加日志
                    //添加到listview_log_listen
                    listview_log_listen_add_data("触发端口1模拟触发");
                }
                if (checkbox_simlate_position_2.Checked)
                {
                    flag_output_2_trigger = true;
                    //添加日志
                    //添加到listview_log_listen
                    listview_log_listen_add_data("触发端口2模拟触发");
                }
                if (checkbox_simlate_position_3.Checked)
                {
                    flag_output_3_trigger = true;
                    //添加日志
                    //添加到listview_log_listen
                    listview_log_listen_add_data("触发端口3模拟触发");
                }
                if (checkbox_simlate_position_4.Checked)
                {
                    flag_output_4_trigger = true;
                    //添加日志
                    //添加到listview_log_listen
                    listview_log_listen_add_data("触发端口4模拟触发");
                }

            }
            catch (Exception)
            {
                //MessageBox.Show("错误：" + ee.ToString(), "提示", MessageBoxButtons.OK);
                //错误显示
                error_message_show();
                //退出程序
                Environment.Exit(0);
            }
            finally
            {
            }

        }
        #endregion


        #region 输出位置一额外定时器tick事件
        /// <summary>
        /// 函数功能：输出位置一额外定时器tick事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timer_output_addition_1_tick(object sender, EventArgs e)
        {
            try
            {

                //如果未达到额定输出次数
                if (addtional_timer_1_amount < 端口侦听控制系统.Properties.Settings.Default.grp_output_1_control_config_additional_amount && flag_output_position_1_in_output == true)
                {
                    output_position_1_output();

                    //额外输出计数器值加一
                    addtional_timer_1_amount++;
                }
                else
                {
                    //额外追加计数器停止
                    timer_output_addition_1.Stop();
                }

            }
            catch (Exception ee)
            {
                MessageBox.Show("错误：" + ee.ToString(), "提示", MessageBoxButtons.OK);

                //退出程序
                Environment.Exit(0);
            }
            finally
            {
            }
        }
        #endregion


        #region 输出位置二额外定时器tick事件
        /// <summary>
        /// 函数功能：输出位置二额外定时器tick事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timer_output_addition_2_tick(object sender, EventArgs e)
        {
            try
            {

                //如果未达到额定输出次数
                if (addtional_timer_2_amount < 端口侦听控制系统.Properties.Settings.Default.grp_output_2_control_config_additional_amount && flag_output_position_2_in_output == true)
                {
                    output_position_2_output();

                    //额外输出计数器值加一
                    addtional_timer_2_amount++;
                }
                else
                {
                    //额外追加计数器停止
                    timer_output_addition_2.Stop();
                }

            }
            catch (Exception ee)
            {
                MessageBox.Show("错误：" + ee.ToString(), "提示", MessageBoxButtons.OK);

                //退出程序
                Environment.Exit(0);
            }
            finally
            {
            }
        }
        #endregion


        #region 输出位置三额外定时器tick事件
        /// <summary>
        /// 函数功能：输出位置三额外定时器tick事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timer_output_addition_3_tick(object sender, EventArgs e)
        {
            try
            {

                //如果未达到额定输出次数
                if (addtional_timer_3_amount < 端口侦听控制系统.Properties.Settings.Default.grp_output_3_control_config_additional_amount && flag_output_position_3_in_output == true)
                {
                    output_position_3_output();

                    //额外输出计数器值加一
                    addtional_timer_3_amount++;
                }
                else
                {
                    //额外追加计数器停止
                    timer_output_addition_3.Stop();
                }

            }
            catch (Exception ee)
            {
                MessageBox.Show("错误：" + ee.ToString(), "提示", MessageBoxButtons.OK);

                //退出程序
                Environment.Exit(0);
            }
            finally
            {
            }
        }
        #endregion


        #region 输出位置四额外定时器tick事件
        /// <summary>
        /// 函数功能：输出位置四额外定时器tick事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timer_output_addition_4_tick(object sender, EventArgs e)
        {
            try
            {

                //如果未达到额定输出次数
                if (addtional_timer_4_amount < 端口侦听控制系统.Properties.Settings.Default.grp_output_4_control_config_additional_amount && flag_output_position_4_in_output == true)
                {
                    output_position_4_output();

                    //额外输出计数器值加一
                    addtional_timer_4_amount++;
                }
                else
                {
                    //额外追加计数器停止
                    timer_output_addition_4.Stop();
                }

            }
            catch (Exception ee)
            {
                MessageBox.Show("错误：" + ee.ToString(), "提示", MessageBoxButtons.OK);

                //退出程序
                Environment.Exit(0);
            }
            finally
            {
            }
        }
        #endregion



        #endregion


        #region 关闭窗体
        /// <summary>
        /// 函数功能：关闭窗体
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void frm_main_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                //如果正在监听中
                if (flag_in_listening)
                {
                    MessageBox.Show("正在侦听中，无法退出程序", "提示", MessageBoxButtons.OK);
                    //取消关闭
                    e.Cancel = true;
                }
                //如果输出位置一输出中
                else if(flag_output_position_1_in_output)
                {
                    MessageBox.Show("端口一正在输出中，无法退出程序", "提示", MessageBoxButtons.OK);
                    //取消关闭
                    e.Cancel = true;
                }
                //如果输出位置二输出中
                else if (flag_output_position_2_in_output)
                {
                    MessageBox.Show("端口二正在输出中，无法退出程序", "提示", MessageBoxButtons.OK);
                    //取消关闭
                    e.Cancel = true;
                }
                //如果输出位置三输出中
                else if (flag_output_position_3_in_output)
                {
                    MessageBox.Show("端口三正在输出中，无法退出程序", "提示", MessageBoxButtons.OK);
                    //取消关闭
                    e.Cancel = true;
                }
                //如果输出位置四输出中
                else if (flag_output_position_4_in_output)
                {
                    MessageBox.Show("端口四正在输出中，无法退出程序", "提示", MessageBoxButtons.OK);
                    //取消关闭
                    e.Cancel = true;
                }
                else if (flag_in_simulate)
                {
                    MessageBox.Show("正在模拟触发中，无法退出程序", "提示", MessageBoxButtons.OK);
                    //取消关闭
                    e.Cancel = true;
                }
                else
                {
                    //进行确定
                    if (MessageBox.Show("您确定要退出程序么？", "提示", MessageBoxButtons.OKCancel) == DialogResult.OK)
                    {
                        //退出程序
                        Environment.Exit(0);

                    }
                    else
                    {
                        //取消关闭
                        e.Cancel = true;
                    }
                }
            }
            catch (Exception ee)
            {
                MessageBox.Show("错误：" + ee.ToString(), "提示", MessageBoxButtons.OK);
            }
            finally
            {

            }
        }
        #endregion


        #region 装载窗体
        /// <summary>
        /// 装载窗体时时间
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void frm_main_Load(object sender, EventArgs e)
        {
            try
            {

                //设置鼠标等待
                Cursor.Current = Cursors.WaitCursor;

                //装载窗体时打开自动运行选项
                frm_main_autorun();
                //输出设置颜色
                btn_output_save.ForeColor = Color.Black;


            }
            catch (Exception)
            {
                //MessageBox.Show("错误：" + ee.ToString(), "提示", MessageBoxButtons.OK);
                //错误显示
                error_message_show();
                //退出程序
                Environment.Exit(0);
            }
            finally
            {

                //设置鼠标为箭头
                Cursor.Current = Cursors.Arrow;
            }
        }
        #endregion

        
        #region 输出设置区域更改事件
    

        #region 端口一


        #region tab_control_output_1
        /// <summary>
        /// 函数功能：tabpage 选取变更
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tab_control_output_1_SelectedIndexChanged(object sender, EventArgs e)
        {
            //如果更改，将保存按键变为红色
            btn_output_save.ForeColor = Color.Red;
        }
        #endregion


        #region txtbox_output_1_sp_write_data
        /// <summary>
        /// 函数功能：txtbox值改变
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtbox_output_1_sp_write_data_TextChanged(object sender, EventArgs e)
        {
            //如果更改，将保存按键变为红色
            btn_output_save.ForeColor = Color.Red;
        }
        #endregion


        #region txtbox_output_1_message_data_1
        /// <summary>
        /// 函数功能：txtbox值改变
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtbox_output_1_message_data_1_TextChanged(object sender, EventArgs e)
        {
            //如果更改，将保存按键变为红色
            btn_output_save.ForeColor = Color.Red;
        }
        #endregion


        #region txtbox_output_1_message_data_2
        /// <summary>
        /// 函数功能：txtbox值改变
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtbox_output_1_message_data_2_TextChanged(object sender, EventArgs e)
        {
            //如果更改，将保存按键变为红色
            btn_output_save.ForeColor = Color.Red;
        }
        #endregion


        #region txtbox_output_1_window_data_1
        /// <summary>
        /// 函数功能：txtbox值改变
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtbox_output_1_window_data_1_TextChanged(object sender, EventArgs e)
        {
            //如果更改，将保存按键变为红色
            btn_output_save.ForeColor = Color.Red;
        }
        #endregion


        #region txtbox_output_1_window_data_2
        /// <summary>
        /// 函数功能：txtbox值改变
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtbox_output_1_window_data_2_TextChanged(object sender, EventArgs e)
        {
            //如果更改，将保存按键变为红色
            btn_output_save.ForeColor = Color.Red;
        }
        #endregion

        #endregion


        #region 端口二


        #region tab_control_output_2
        /// <summary>
        /// 函数功能：tabpage 选取变更
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tab_control_output_2_SelectedIndexChanged(object sender, EventArgs e)
        {
            //如果更改，将保存按键变为红色
            btn_output_save.ForeColor = Color.Red;
        }
        #endregion


        #region txtbox_output_2_sp_write_data
        /// <summary>
        /// 函数功能：txtbox值改变
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtbox_output_2_sp_write_data_TextChanged(object sender, EventArgs e)
        {
            //如果更改，将保存按键变为红色
            btn_output_save.ForeColor = Color.Red;
        }
        #endregion


        #region txtbox_output_2_message_data_1
        /// <summary>
        /// 函数功能：txtbox值改变
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtbox_output_2_message_data_1_TextChanged(object sender, EventArgs e)
        {
            //如果更改，将保存按键变为红色
            btn_output_save.ForeColor = Color.Red;
        }
        #endregion


        #region txtbox_output_2_message_data_2
        /// <summary>
        /// 函数功能：txtbox值改变
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtbox_output_2_message_data_2_TextChanged(object sender, EventArgs e)
        {
            //如果更改，将保存按键变为红色
            btn_output_save.ForeColor = Color.Red;
        }
        #endregion


        #region txtbox_output_2_window_data_1
        /// <summary>
        /// 函数功能：txtbox值改变
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtbox_output_2_window_data_1_TextChanged(object sender, EventArgs e)
        {
            //如果更改，将保存按键变为红色
            btn_output_save.ForeColor = Color.Red;
        }
        #endregion


        #region txtbox_output_2_window_data_2
        /// <summary>
        /// 函数功能：txtbox值改变
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtbox_output_2_window_data_2_TextChanged(object sender, EventArgs e)
        {
            //如果更改，将保存按键变为红色
            btn_output_save.ForeColor = Color.Red;
        }
        #endregion


        #endregion


        #region 端口三


        #region tab_control_output_3
        /// <summary>
        /// 函数功能：tabpage 选取变更
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tab_control_output_3_SelectedIndexChanged(object sender, EventArgs e)
        {
            //如果更改，将保存按键变为红色
            btn_output_save.ForeColor = Color.Red;
        }
        #endregion


        #region txtbox_output_3_sp_write_data
        /// <summary>
        /// 函数功能：txtbox值改变
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtbox_output_3_sp_write_data_TextChanged(object sender, EventArgs e)
        {
            //如果更改，将保存按键变为红色
            btn_output_save.ForeColor = Color.Red;
        }
        #endregion


        #region txtbox_output_3_message_data_1
        /// <summary>
        /// 函数功能：txtbox值改变
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtbox_output_3_message_data_1_TextChanged(object sender, EventArgs e)
        {
            //如果更改，将保存按键变为红色
            btn_output_save.ForeColor = Color.Red;
        }
        #endregion


        #region txtbox_output_3_message_data_2
        /// <summary>
        /// 函数功能：txtbox值改变
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtbox_output_3_message_data_2_TextChanged(object sender, EventArgs e)
        {
            //如果更改，将保存按键变为红色
            btn_output_save.ForeColor = Color.Red;
        }
        #endregion


        #region txtbox_output_3_window_data_1
        /// <summary>
        /// 函数功能：txtbox值改变
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtbox_output_3_window_data_1_TextChanged(object sender, EventArgs e)
        {
            //如果更改，将保存按键变为红色
            btn_output_save.ForeColor = Color.Red;
        }
        #endregion


        #region txtbox_output_3_window_data_2
        /// <summary>
        /// 函数功能：txtbox值改变
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtbox_output_3_window_data_2_TextChanged(object sender, EventArgs e)
        {
            //如果更改，将保存按键变为红色
            btn_output_save.ForeColor = Color.Red;
        }
        #endregion


        #endregion


        #region 端口四


        #region tab_control_output_4
        /// <summary>
        /// 函数功能：tabpage 选取变更
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tab_control_output_4_SelectedIndexChanged(object sender, EventArgs e)
        {
            //如果更改，将保存按键变为红色
            btn_output_save.ForeColor = Color.Red;
        }
        #endregion


        #region txtbox_output_4_sp_write_data
        /// <summary>
        /// 函数功能：txtbox值改变
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtbox_output_4_sp_write_data_TextChanged(object sender, EventArgs e)
        {
            //如果更改，将保存按键变为红色
            btn_output_save.ForeColor = Color.Red;
        }
        #endregion


        #region txtbox_output_4_message_data_1
        /// <summary>
        /// 函数功能：txtbox值改变
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtbox_output_4_message_data_1_TextChanged(object sender, EventArgs e)
        {
            //如果更改，将保存按键变为红色
            btn_output_save.ForeColor = Color.Red;
        }
        #endregion


        #region txtbox_output_4_message_data_2
        /// <summary>
        /// 函数功能：txtbox值改变
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtbox_output_4_message_data_2_TextChanged(object sender, EventArgs e)
        {
            //如果更改，将保存按键变为红色
            btn_output_save.ForeColor = Color.Red;
        }
        #endregion

        
        #region txtbox_output_4_window_data_1
        /// <summary>
        /// 函数功能：txtbox值改变
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtbox_output_4_window_data_1_TextChanged(object sender, EventArgs e)
        {
            //如果更改，将保存按键变为红色
            btn_output_save.ForeColor = Color.Red;
        }
        #endregion


        #region txtbox_output_4_window_data_2
        /// <summary>
        /// 函数功能：txtbox值改变
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtbox_output_4_window_data_2_TextChanged(object sender, EventArgs e)
        {
            //如果更改，将保存按键变为红色
            btn_output_save.ForeColor = Color.Red;
        }
        #endregion

     
       

        #endregion


        #endregion


        #region listview_log_listen 事件


        #region  listview_log_listen 鼠标点击事件
        /// <summary>
        /// 函数功能：listview_log_listen 鼠标点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void listview_log_listen_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                //弹出侦听右键菜单
                cms_log_listen.Show(Cursor .Position .X  ,Cursor .Position .Y);
            }
        }
        #endregion

      
        #endregion


        #region listview_log_output 事件


        #region  listview_log_output 鼠标点击事件
        /// <summary>
        /// 函数功能：listview_log_output 鼠标点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void listview_log_output_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                cms_log_output.Show(Cursor .Position .X  , Cursor .Position .Y );
            }
        }
        #endregion


        #endregion


        #endregion


    }
}

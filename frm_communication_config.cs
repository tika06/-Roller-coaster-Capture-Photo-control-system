using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace 端口侦听控制系统
{
    public partial class frm_communication_config : Form
    {
        //传递参数
        frm_main my_frm_main;

        public frm_communication_config(frm_main my_frm)
        {
            InitializeComponent();

            //接受参数
            my_frm_main = my_frm ;

            //界面居中显示
            this.StartPosition = FormStartPosition.CenterScreen;
        }

       

        #region 按键功能


        #region 保存
        /// <summary>
        /// 函数功能：保存
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_communication_config_save_Click(object sender, EventArgs e)
        {
            try
            {
                //错误提示控件
                ToolTip my_tooltip = new ToolTip();

                //检测端口名（不为空）
                if (txtbox_communication_config_port.Text.Trim().Length <= 0)
                {
                    //设为输入焦点
                    txtbox_communication_config_port.Focus();
                    my_tooltip.Show("端口不能为空", txtbox_communication_config_port, 3000);
                    return;
                }

                //检测波特率
                int baud_rate;
                if (!int.TryParse(txtbox_communication_config_baud_rate.Text.Trim().ToString(), out  baud_rate))
                {
                    //设为输入焦点
                    txtbox_communication_config_baud_rate.Focus();
                    my_tooltip.Show("波特率必须为整数", txtbox_communication_config_baud_rate, 3000);
                    return;
                }

                //检测数据位（为数字）
                int data_bit;
                if (!int.TryParse(txtbox_communication_config_databit.Text.Trim().ToString(), out  data_bit))
                {
                    //设为输入焦点
                    txtbox_communication_config_databit.Focus();
                    my_tooltip.Show("数据位必须为整数", txtbox_communication_config_databit, 3000);
                    return;
                }

                //检测数据位（5到8之间）
                if (data_bit < 5 || data_bit > 8)
                {
                    //设为输入焦点
                    txtbox_communication_config_databit.Focus();
                    my_tooltip.Show("数据位必须在5位到8位之间", txtbox_communication_config_databit, 3000);
                    return;
                }

               
                //检测校验位
                if (cmb_communication_config_parity.Text.Trim().ToString().Length == 0)
                {
                    //设为输入焦点
                    cmb_communication_config_parity.Focus();
                    my_tooltip.Show("请设置校验位", cmb_communication_config_parity, 3000);
                    return;
                }

               
                //检测停止位
                if (cmb_communication_config_stopbit.Text.Trim().ToString().Length == 0)
                {
                    //设为输入焦点
                    cmb_communication_config_stopbit.Focus();
                    my_tooltip.Show("请设置停止位", cmb_communication_config_stopbit, 3000);
                    return;
                }


                //注销资源
                my_tooltip.Dispose();


                //保存设置

            
                switch (this.Tag.ToString())
                {
                    //侦听端口
                    case "listening":
                        {
                            //端口名
                            端口侦听控制系统.Properties.Settings.Default.grp_listening_comunication_config_port_name = txtbox_communication_config_port.Text.Trim().ToString();
                            //波特率
                            端口侦听控制系统.Properties.Settings.Default.grp_listening_comunication_config_baud_rate = baud_rate;
                            //数据位
                            端口侦听控制系统.Properties.Settings.Default.grp_listening_comunication_config_databit = data_bit;
                            //校验位
                            端口侦听控制系统.Properties.Settings.Default.grp_listening_comunication_config_parity = cmb_communication_config_parity.Text.Trim().ToString();
                            //停止位
                            端口侦听控制系统.Properties.Settings.Default.grp_listening_comunication_config_stopbit = cmb_communication_config_stopbit.Text.Trim().ToString();
                            //保存设置
                            端口侦听控制系统.Properties.Settings.Default.Save();


                            //更新端口显示标签
                            my_frm_main.lbl_listening_port_name.Text  = txtbox_communication_config_port.Text.Trim().ToString();

                            break;
                        }
                    //触发位置一
                    case "grp_output_1":
                        {
                            //端口名
                            端口侦听控制系统.Properties.Settings.Default.grp_output_1_port_comunication_config_port_name = txtbox_communication_config_port.Text.Trim().ToString();
                            //波特率
                            端口侦听控制系统.Properties.Settings.Default.grp_output_1_port_comunication_config_baud_rate = baud_rate;
                            //数据位
                            端口侦听控制系统.Properties.Settings.Default.grp_output_1_port_comunication_config_databit = data_bit;
                            //校验位
                            端口侦听控制系统.Properties.Settings.Default.grp_output_1_port_comunication_config_parity = cmb_communication_config_parity.Text.Trim().ToString();
                            //停止位
                            端口侦听控制系统.Properties.Settings.Default.grp_output_1_port_comunication_config_stopbit = cmb_communication_config_stopbit.Text.Trim().ToString();
                            //保存设置
                            端口侦听控制系统.Properties.Settings.Default.Save();

                            //更新端口显示标签
                            my_frm_main.lbl_output_1_sp_port_name.Text = txtbox_communication_config_port.Text.Trim().ToString();

                            break;
                        }
                        
                    //触发位置二
                    case "grp_output_2":
                        {
                            //端口名
                            端口侦听控制系统.Properties.Settings.Default.grp_output_2_port_comunication_config_port_name = txtbox_communication_config_port.Text.Trim().ToString();
                            //波特率
                            端口侦听控制系统.Properties.Settings.Default.grp_output_2_port_comunication_config_baud_rate = baud_rate;
                            //数据位
                            端口侦听控制系统.Properties.Settings.Default.grp_output_2_port_comunication_config_databit = data_bit;
                            //校验位
                            端口侦听控制系统.Properties.Settings.Default.grp_output_2_port_comunication_config_parity = cmb_communication_config_parity.Text.Trim().ToString();
                            //停止位
                            端口侦听控制系统.Properties.Settings.Default.grp_output_2_port_comunication_config_stopbit = cmb_communication_config_stopbit.Text.Trim().ToString();
                            //保存设置
                            端口侦听控制系统.Properties.Settings.Default.Save();

                            //更新端口显示标签
                            my_frm_main.lbl_output_2_sp_port_name.Text  = txtbox_communication_config_port.Text.Trim().ToString();


                            break;
                        }
                    //触发位置三
                    case "grp_output_3":
                        {
                            //端口名
                            端口侦听控制系统.Properties.Settings.Default.grp_output_3_port_comunication_config_port_name = txtbox_communication_config_port.Text.Trim().ToString();
                            //波特率
                            端口侦听控制系统.Properties.Settings.Default.grp_output_3_port_comunication_config_baud_rate = baud_rate;
                            //数据位
                            端口侦听控制系统.Properties.Settings.Default.grp_output_3_port_comunication_config_databit = data_bit;
                            //校验位
                            端口侦听控制系统.Properties.Settings.Default.grp_output_3_port_comunication_config_parity = cmb_communication_config_parity.Text.Trim().ToString();
                            //停止位
                            端口侦听控制系统.Properties.Settings.Default.grp_output_3_port_comunication_config_stopbit = cmb_communication_config_stopbit.Text.Trim().ToString();
                            //保存设置
                            端口侦听控制系统.Properties.Settings.Default.Save();

                            //更新端口显示标签
                            my_frm_main.lbl_output_3_sp_port_name.Text  = txtbox_communication_config_port.Text.Trim().ToString();


                            break;
                        }
                    //触发位置四
                    case "grp_output_4":
                        {
                            //端口名
                            端口侦听控制系统.Properties.Settings.Default.grp_output_4_port_comunication_config_port_name = txtbox_communication_config_port.Text.Trim().ToString();
                            //波特率
                            端口侦听控制系统.Properties.Settings.Default.grp_output_4_port_comunication_config_baud_rate = baud_rate;
                            //数据位
                            端口侦听控制系统.Properties.Settings.Default.grp_output_4_port_comunication_config_databit = data_bit;
                            //校验位
                            端口侦听控制系统.Properties.Settings.Default.grp_output_4_port_comunication_config_parity = cmb_communication_config_parity.Text.Trim().ToString();
                            //停止位
                            端口侦听控制系统.Properties.Settings.Default.grp_output_4_port_comunication_config_stopbit = cmb_communication_config_stopbit.Text.Trim().ToString();
                            //保存设置
                            端口侦听控制系统.Properties.Settings.Default.Save();

                            //更新端口显示标签
                            my_frm_main.lbl_output_4_sp_port_name.Text = txtbox_communication_config_port.Text.Trim().ToString();


                            break;
                        }



                }
                //关闭窗体
                this.Close();
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
      

        #region 取消
        /// <summary>
        /// 函数功能：取消
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_communication_config_cancel_Click(object sender, EventArgs e)
        {
            try
            {
                //关闭窗体
                this.Close();
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


    }
}

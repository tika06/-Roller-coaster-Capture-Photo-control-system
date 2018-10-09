using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace 端口侦听控制系统
{
    public partial class frm_control_config : Form
    {
        //传递窗体
        frm_main my_frm_main;

        public frm_control_config(frm_main my_frm)
        {
            InitializeComponent();
            //接受窗体
            my_frm_main = my_frm;
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
        private void btn_control_config_save_Click(object sender, EventArgs e)
        {
            try
            {
                //错误显示控件
                ToolTip  my_tooltip = new ToolTip ();

                //检测连拍计时器是否为整数
                int series_short_timer;
                if (!int.TryParse(txtbox_control_config_series_shoot_timer.Text.Trim().ToString(), out  series_short_timer))
                {
                    //设置为输入焦点
                    txtbox_control_config_series_shoot_timer.Focus();
                    my_tooltip.Show("连拍计时器必须为整数", txtbox_control_config_series_shoot_timer, 3000);
                    return;
                }


                //连续计时器必须大于1000
                if (series_short_timer < 1000)
                {
                    //设置为输入焦点
                    txtbox_control_config_series_shoot_timer.Focus();
                    my_tooltip.Show("连续计时器不能小于1000", txtbox_control_config_series_shoot_timer, 3000);
                    return;
                }


                //检测拍摄延迟是否为整数
                int shoot_delay = 0;
                if (txtbox_control_config_shoot_delay.Text.Trim().ToString().Length > 0)
                {
                    
                    if (!int.TryParse(txtbox_control_config_shoot_delay.Text.Trim().ToString(), out  shoot_delay))
                    {
                        //设置为输入焦点
                        txtbox_control_config_shoot_delay.Focus();
                        my_tooltip.Show("拍摄延迟必须为整数", txtbox_control_config_shoot_delay, 3000);
                        return;
                    }
                }               

              
                //如果忽略次数不为空
                if (txtbox_control_config_series_shoot_neglect.Text.Trim().Length > 0)
                {
                    //检测拍摄忽略是否为整数
                    //拍摄忽略次数
                    int shoot_neglect_1;
                    int shoot_neglect_2;

                    //前一组的拍摄忽略次数（右端）
                    int last_shoot_neglect = 0;
                    //分割数组
                    string[] temp_1 = txtbox_control_config_series_shoot_neglect.Text.Trim().Split('/');
                    //检查每一个分割组是否正确
                    for (int i = 0; i < temp_1.Length; i++)
                    {
                        string[] temp_2 = temp_1[i].Split('-');
                        //检查是否恰好是两组
                        if (temp_2.Length != 2)
                        {
                            //设置为输入焦点
                            txtbox_control_config_series_shoot_neglect.Focus();
                            my_tooltip.Show("拍摄忽略次数必须以两个数字为一个区间，中间用横杠连接", txtbox_control_config_series_shoot_neglect, 3000);
                            return;
                        }


                        if (!int.TryParse(temp_2[0], out  shoot_neglect_1))
                        {
                            //设置为输入焦点
                            txtbox_control_config_series_shoot_neglect.Focus();
                            my_tooltip.Show("拍摄忽略次数为非数字，或者其格式不正确", txtbox_control_config_series_shoot_neglect,3000);
                            return;
                        }

                        if (!int.TryParse(temp_2[1], out  shoot_neglect_2))
                        {
                            //设置为输入焦点
                            txtbox_control_config_series_shoot_neglect.Focus();
                            my_tooltip.Show("拍摄忽略次数为非数字，或者其格式不正确", txtbox_control_config_series_shoot_neglect, 3000);
                            return;
                        }

                        //检查排列顺序是否正确（本组）
                        if (shoot_neglect_1  > shoot_neglect_2)
                        {
                            //设置为输入焦点
                            txtbox_control_config_series_shoot_neglect.Focus();
                            my_tooltip.Show("同一区间的拍摄忽略次数必须从小到大有序排列", txtbox_control_config_series_shoot_neglect, 3000);
                            return;
                        }

                        //检查排列顺序是否正确（本组和前组）
                        if (last_shoot_neglect > shoot_neglect_1 )
                        {
                            //设置为输入焦点
                            txtbox_control_config_series_shoot_neglect.Focus();
                            my_tooltip.Show("不同区间的拍摄忽略次数必须从小到大有序排列", txtbox_control_config_series_shoot_neglect, 3000);
                            return;
                        }

                        //保存本组的拍摄忽略次数（右端）
                        last_shoot_neglect = shoot_neglect_2;
                    }
                }


                //额外追加输出序号必须为整数
                int series_short_additional_number;
                if (!int.TryParse(txtbox_control_config_series_shoot_additional_number .Text .Trim ().ToString (), out  series_short_additional_number))
                {
                    //设置为输入焦点
                    txtbox_control_config_series_shoot_additional_number.Focus();
                    my_tooltip.Show("额外追加序号必须为整数", txtbox_control_config_series_shoot_additional_number, 3000);
                    return;
                }

                //额外追加输出数量必须为整数
                int series_short_additional_amount;
                if (!int.TryParse(txtbox_control_config_series_shoot_additional_amount.Text.Trim().ToString(), out  series_short_additional_amount))
                {
                    //设置为输入焦点
                    txtbox_control_config_series_shoot_additional_amount.Focus();
                    my_tooltip.Show("额外追加输出数量必须为整数", txtbox_control_config_series_shoot_additional_amount, 3000);
                    return;
                }

                //额外追加输出间隔必须为整数
                int series_short_additional_interval;
                if (!int.TryParse(txtbox_control_config_series_shoot_additional_interval.Text.Trim().ToString(), out  series_short_additional_interval))
                {
                    //设置为输入焦点
                    txtbox_control_config_series_shoot_additional_interval.Focus();
                    my_tooltip.Show("额外追加输出间隔必须为整数", txtbox_control_config_series_shoot_additional_interval, 3000);
                    return;
                }

                //注销资源
                my_tooltip.Dispose();


                //保存设置

                switch (this.Tag.ToString())
                {
                   
                    //触发位置一
                    case "grp_output_1":
                        {
                            //拍摄延时
                            端口侦听控制系统.Properties.Settings.Default.grp_output_1_control_config_shoot_delay = shoot_delay;
                            //连拍计时器
                            端口侦听控制系统.Properties.Settings.Default.grp_output_1_control_config_series_shoot_timer = series_short_timer;
                            //拍摄忽略(去除空格)
                            端口侦听控制系统.Properties.Settings.Default.grp_output_1_control_config_shoot_neglect = txtbox_control_config_series_shoot_neglect .Text .Trim ().ToString ().Replace (" ","");
                            //额外追加输出序号
                            端口侦听控制系统.Properties.Settings.Default.grp_output_1_control_config_additional_number = series_short_additional_number ;
                            //额外追加输出数量
                            端口侦听控制系统.Properties.Settings.Default.grp_output_1_control_config_additional_amount = series_short_additional_amount;
                            //额外追加输出间隔
                            端口侦听控制系统.Properties.Settings.Default.grp_output_1_control_config_additional_interval = series_short_additional_interval;
                            //保存设置
                            端口侦听控制系统.Properties.Settings.Default.Save();
                            break;
                        }

                    //触发位置二
                    case "grp_output_2":
                        {
                            //拍摄延时
                            端口侦听控制系统.Properties.Settings.Default.grp_output_2_control_config_shoot_delay = shoot_delay;
                            //连拍计时器
                            端口侦听控制系统.Properties.Settings.Default.grp_output_2_control_config_series_shoot_timer = series_short_timer;
                            //拍摄忽略(去除空格)
                            端口侦听控制系统.Properties.Settings.Default.grp_output_2_control_config_shoot_neglect = txtbox_control_config_series_shoot_neglect.Text.Trim().ToString().Replace(" ", "");
                            //额外追加输出序号
                            端口侦听控制系统.Properties.Settings.Default.grp_output_2_control_config_additional_number = series_short_additional_number;
                            //额外追加输出数量
                            端口侦听控制系统.Properties.Settings.Default.grp_output_2_control_config_additional_amount = series_short_additional_amount;
                            //额外追加输出间隔
                            端口侦听控制系统.Properties.Settings.Default.grp_output_2_control_config_additional_interval = series_short_additional_interval;
                            //保存设置
                            端口侦听控制系统.Properties.Settings.Default.Save();
                            break;
                        }
                    //触发位置三
                    case "grp_output_3":
                        {
                            //拍摄延时
                            端口侦听控制系统.Properties.Settings.Default.grp_output_3_control_config_shoot_delay = shoot_delay;
                            //连拍计时器
                            端口侦听控制系统.Properties.Settings.Default.grp_output_3_control_config_series_shoot_timer = series_short_timer;
                            //拍摄忽略(去除空格)
                            端口侦听控制系统.Properties.Settings.Default.grp_output_3_control_config_shoot_neglect = txtbox_control_config_series_shoot_neglect.Text.Trim().ToString().Replace(" ", "");
                            //额外追加输出序号
                            端口侦听控制系统.Properties.Settings.Default.grp_output_3_control_config_additional_number = series_short_additional_number;
                            //额外追加输出数量
                            端口侦听控制系统.Properties.Settings.Default.grp_output_3_control_config_additional_amount = series_short_additional_amount;
                            //额外追加输出间隔
                            端口侦听控制系统.Properties.Settings.Default.grp_output_3_control_config_additional_interval = series_short_additional_interval;
                            //保存设置
                            端口侦听控制系统.Properties.Settings.Default.Save();
                            break;
                        }
                    //触发位置四
                    case "grp_output_4":
                        {
                            //拍摄延时
                            端口侦听控制系统.Properties.Settings.Default.grp_output_4_control_config_shoot_delay = shoot_delay;
                            //连拍计时器
                            端口侦听控制系统.Properties.Settings.Default.grp_output_4_control_config_series_shoot_timer = series_short_timer;
                            //拍摄忽略(去除空格)
                            端口侦听控制系统.Properties.Settings.Default.grp_output_4_control_config_shoot_neglect = txtbox_control_config_series_shoot_neglect.Text.Trim().ToString().Replace(" ", "");
                            //额外追加输出序号
                            端口侦听控制系统.Properties.Settings.Default.grp_output_4_control_config_additional_number = series_short_additional_number;
                            //额外追加输出数量
                            端口侦听控制系统.Properties.Settings.Default.grp_output_4_control_config_additional_amount = series_short_additional_amount;
                            //额外追加输出间隔
                            端口侦听控制系统.Properties.Settings.Default.grp_output_4_control_config_additional_interval = series_short_additional_interval;
                            //保存设置
                            端口侦听控制系统.Properties.Settings.Default.Save();
                            break;
                        }



                }
                //更新控制状态
                my_frm_main.update_output_state();
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
        private void btn_control_config_cancel_Click(object sender, EventArgs e)
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

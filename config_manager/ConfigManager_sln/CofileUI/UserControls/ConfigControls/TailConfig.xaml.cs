using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CofileUI.UserControls.ConfigControls
{
	/// <summary>
	/// TailConfig.xaml에 대한 상호 작용 논리
	/// </summary>
	public partial class TailConfig : UserControl
	{
		public TailConfig()
		{
			InitializeComponent();
		}


		class SamOptions : Options
		{
			string[] _options = new string[]
			{
				// comm_option
				"input_dir"
				, "input_ext"
				, "output_dir"
				, "output_ext"
				, "sid"
				, "tail_type"
				, "interval"
				, "no_inform"
				, "input_filter"
				, "input_filter_ex"
				, "shutdown_time"
				, "zero_byte_yn"
				, "no_access_sentence"
				, "file_reserver_yn"
				, "reg_yn"
				
				// enc_inform
				, "item"
				, "pattern"
				, "delimiter"
				, "sub_left_len"
				, "sub_right_len"
				, "enc_pattern"
				, "jumin_check_yn"
			};
			string[] detailOptions = new string[]
			{
				// comm_option
				"암/복호화 할 입력 로그 파일이 위치하는 경로"
				, "암/복호화 할 입력 로그파일의 확장자"
				, "암/복호화 후 출력 경로"
				, "암/복호화 후 덧붙일 확장자"
				, "DB SID 이름"
				, "cofiletail 암/복호화 방식"
				, "암호화시, 입력 폴더의 감시하는 주기"
				, "tail_type이 PATTERN일 경우 패턴을 정하는 개수"
				, "암/복호화 할 파일에 대한 패턴, 정규표현식 지원 (input_ext의 옵션보다 우선순위가 높다)"
				, "input_filter_ex"
				, "자식 데몬들을 특정 시간 후 종료한다"
				, "데몬 시작시 파일크기가 0인 파일에 대해서 암/복호화 유/무 (true면 0byte파일도 감시를 한다)"
				, "no_access_sentence"
				, "file_reserver_yn"
				, "reg_yn"
				
				// enc_inform
				, "암/복호화에 사용할 ITEM 명"
				, "감시하고자 하는 pattern (정규표현식으로 작성)"
				, "구분자 (암호화: 암호문의 앞뒤로 구분자를 붙인다, 복호화: 구분자로 암호문인지 판단한다)"
				, "감시한 패턴에서 왼쪽에서 제외할 크기"
				, "감시한 패턴에서 오른쪽에서 제외할 크기"
				, "enc_pattern"
				, "jumin_check_yn"
			};
			enum Options
			{
				// comm_option
				input_dir = 0
				, input_ext
				, output_dir
				, output_ext
				, sid
				, tail_type
				, interval
				, no_inform
				, input_filter
				, input_filter_ex
				, shutdown_time
				, zero_byte_yn
				, no_access_sentence
				, file_reserver_yn
				, reg_yn

				// enc_inform
				, item
				, pattern
				, delimiter
				, sub_left_len
				, sub_right_len
				, enc_pattern
				, jumin_check_yn
			}
			class OptionInfo
			{
				public string Key { get; set; }
				public string Detail { get; set; }
				//public Options Number { get; set; }
				public Options Index { get; set; }
			}
			Dictionary<string, OptionInfo> dic_options = new Dictionary<string, OptionInfo>();
			public void InitDic()
			{
				for(int i = 0; i < _options.Length; i++)
				{
					dic_options.Add(_options[i], new OptionInfo()
					{
						Key = _options[i]
							,
						Detail = detailOptions[i]
							//, Number = GetOption(_options[i])
							,
						Index = (Options)i
					}
					);
				}
			}
		}
	}
}

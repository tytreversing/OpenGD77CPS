using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;

using System.Windows.Forms;

namespace DMR
{
    public partial class Examination : Form
    {

        private int currentQuestion = 0;
        private static ExamQuestion examQuestion1 = new ExamQuestion("Для использования без лицензии в России легальны диапазоны",
            "LPD и PMR без ограничений",
            "LPD и PMR с ограничениями по мощности",
            "Все УКВ с ограничениями по мощности",
            "Никакие", 2);
        private static ExamQuestion examQuestion2 = new ExamQuestion("Для настройки MD-9600 на частоту дальнобойщиков нужно",
            "Установить правильную прошивку",
            "Задать правильные калибровки",
            "Получить лицензию",
            "Включить голову и купить рацию CB-диапазона", 4);
        private static ExamQuestion examQuestion3 = new ExamQuestion("Симметрирующий четвертьволновый стакан устанавливается",
            "На фидере со стороны антенны открытым концом к ней",
            "На фидере со стороны антенны открытым концом от нее",
            "На фидере со стороны трансивера в любой ориентации",
            "На фидере со стороны трансивера открытым концом к нему", 1);
        private static ExamQuestion examQuestion4 = new ExamQuestion("Разнос частот приема и передачи используется для",
            "Уменьшения помех",
            "Увеличения дальности уверенного приема",
            "Работы с ретранслятором",
            "Повышения КПД передатчика", 3);
        private static ExamQuestion examQuestion5 = new ExamQuestion("Четвертая категория радиолюбительской лицензии в России",
            "Дает право работы на всех диапазонах с мощностью до 5 Вт",
            "Дает право работы только на УКВ с мощностью до 5 Вт",
            "Дает право работать только на УКВ с мощностью до 10 Вт",
            "Дает право работать только на КВ", 2);
        private static ExamQuestion examQuestion6 = new ExamQuestion("Идеальным КСВ для антенно-фидерного тракта является",
            "Максимально достижимое значение",
            "Значение как можно ближе к 0",
            "Значение не более 1",
            "Значение, минимально превышающее 1", 4);
        private static ExamQuestion examQuestion7 = new ExamQuestion("Передача вне безлицензионных и радиолюбительских диапазонов",
            "Запрещена в любом случае",
            "Разрешена в экстренных ситуациях",
            "Разрешена при мощности не более 0,1 Вт",
            "Запрещена без регистрации радиостанции", 2);
        private static ExamQuestion examQuestion8 = new ExamQuestion("Для приема сигнала в сети DMR необходимо",
            "Настроить частоту, таймслот, колоркод, субтон",
            "Настроить DMR ID, частоту, таймслот",
            "Настроить частоту, субтон, разговорную группу",
            "Настроить частоту, таймслот, колоркод, разговорную группу", 4);
        private static ExamQuestion examQuestion9 = new ExamQuestion("Выберите российский позывной",
            "VK3KYY",
            "9RU0AA",
            "UB0AHS",
            "TD7RUS", 3);
        private static ExamQuestion examQuestion10 = new ExamQuestion("Частным случаем антенн бегущей волны является",
            "Антенна Ground Plane",
            "J-антенна",
            "Вибратор Пистолькорса",
            "Антенна Уда-Яги", 4);
        private static ExamQuestion examQuestion11 = new ExamQuestion("Использование субтонов в аналоговой связи позволяет",
            "Снизить уровень помех",
            "Принимать передачу только от ограниченного круга абонентов",
            "Зашифровать передаваемый сигнал",
            "Улучшить разборчивость сигнала", 2);
        private static ExamQuestion examQuestion12 = new ExamQuestion("Использование шифрования в радиолюбительской практике",
            "Категорически запрещено",
            "Разрешено для радиолюбителей 1-й категории",
            "Не рекомендуется",
            "Не нормируется", 1);
        private static ExamQuestion examQuestion13 = new ExamQuestion("Чувствительность приемника - это",
            "Минимальный уровень сигнала, необходимый для открытия шумоподавителя",
            "Минимальная мощность передатчика, необходимая для уверенного приема",
            "Минимальный уровень эталонно-модулированного сигнала, при котором обеспечивается заданное соотношение сигнал/шум на выходе",
            "Суммарное усиление сигнала его каскадами", 3);
        private static ExamQuestion examQuestion14 = new ExamQuestion("Для улучшения приема станции нужно",
            "Уменьшить уровень настройки шумоподавителя, если это возможно",
            "Использовать резонансную направленную антенну",
            "Использовать более длинную антенну, но не более 3 длин волны",
            "Использовать широкополосную антенну", 2);
        private static ExamQuestion examQuestion15 = new ExamQuestion("Приемник прямого преобразования",
            "Имеет один гетеродин",
            "Не имеет гетеродина",
            "Может иметь или не иметь гетеродин",
            "Это синоним приемника прямого усиления", 1);

        private static ExamQuestion[] exams = new ExamQuestion[]
            {examQuestion1, examQuestion2, examQuestion3, examQuestion4, examQuestion5,
             examQuestion6, examQuestion7, examQuestion8, examQuestion9, examQuestion10,
             examQuestion11, examQuestion12, examQuestion13, examQuestion14, examQuestion15
            };

        private int numQuestions = exams.Length;

        private int correctAnswers = 0;

        public Examination()
        {
            InitializeComponent();
            base.Icon = Icon.ExtractAssociatedIcon(System.Windows.Forms.Application.ExecutablePath);
        }

        private void drawStep(int stepNumber)
        {
            rbAns1.Checked = false;
            rbAns1.Text = exams[stepNumber].answer1;
            rbAns2.Checked = false;
            rbAns2.Text = exams[stepNumber].answer2;
            rbAns3.Checked = false;
            rbAns3.Text = exams[stepNumber].answer3;
            rbAns4.Checked = false;
            rbAns4.Text = exams[stepNumber].answer4;
            lblQuestion.Text = exams[stepNumber].question;
        }

        private void Examination_Load(object sender, EventArgs e)
        {
            drawStep(currentQuestion);
        }

        private bool checkAnswer(int num)
        {
            switch (exams[num].rightAnswer)
            {
                case 1:
                    return rbAns1.Checked;
                case 2:
                    return rbAns2.Checked;
                case 3:
                    return rbAns3.Checked;
                case 4:
                    return rbAns4.Checked;
                default:
                    return false;
            }
        }

        private void actionButton_Click(object sender, EventArgs e)
        {
            if (checkAnswer(currentQuestion))
            {
                correctAnswers++;
            }
            currentQuestion++;
            if (currentQuestion == numQuestions) //вопросы закончились
            {
                if (correctAnswers > (numQuestions * 0.8f))
                {
                    IniFileUtils.WriteProfileString("Setup", "HardwareID", MainForm.GetWindowsId());
                    MessageBox.Show("Теперь редактор калибровок активен в меню!", "Тест пройден!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.Close();
                }
                else
                {
                    int i = IniFileUtils.getProfileIntWithDefault("Setup", "InitializingBlocks", 0) + 1;
                    IniFileUtils.WriteProfileInt("Setup", "InitializingBlocks", i);
                    MessageBox.Show("Слишком много ошибок при прохождении теста. Редактор калибровок остается заблокированным.", "Тест не пройден!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this.Close();
                }
            }
            else
                drawStep(currentQuestion);
        }
    }




    public class ExamQuestion
    {
        public string question { get; set; }
        public string answer1 { get; set; }
        public string answer2 { get; set; }
        public string answer3 { get; set; }
        public string answer4 { get; set; }
        public int rightAnswer { get; set; }

        public ExamQuestion(string q, string a1, string a2, string a3, string a4, int ra)
        {
            question = q;
            answer1 = a1;
            answer2 = a2;
            answer3 = a3;
            answer4 = a4;
            rightAnswer = ra;
        }

        public ExamQuestion()
        {
            question = "";
            answer1 = "";
            answer2 = "";
            answer3 = "";
            answer4 = "";
            rightAnswer = 0;
        }
    }
}

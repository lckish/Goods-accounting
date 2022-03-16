using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace Shklyaev_3Lab
{
    abstract public class Tovar : IComparable
    {
        public int id;
        public bool isDeleted;
        public byte type_record;
        protected string TovarName;
        protected DateTime date;
        protected int balance;

        abstract public void setTovar(string name);
        abstract public void setBalance(int balance);
        abstract public void setDate(DateTime date);

        abstract public string getTovar();
        abstract public int getBalance();
        abstract public DateTime getDate();
        abstract public string getNameClass();

        public int CompareTo(object o)
        {
            if (this.date == (((Tovar)o).date))
                return this.TovarName.CompareTo(((Tovar)o).TovarName);
            else return DateTime.Compare(this.date, (((Tovar)o).date));
        }
    }

    public class Description : Tovar
    {
        protected string depiction;
        protected string name_class = "Учет товара с описанием";
        public Description(int id, bool isDeleted, byte type_record, string tovar, DateTime date, int balance, string description)
        {
            this.isDeleted = isDeleted;
            this.type_record = type_record;
            this.id = id;
            this.TovarName = tovar;
            this.date = date;
            this.balance = balance;
            this.depiction = description;
        }

        override public void setTovar(string tovar) { this.TovarName = tovar; }
        override public void setDate(DateTime date) { this.date = date; }
        override public void setBalance(int balance) { this.balance = balance; }
        override public string getTovar() { return this.TovarName; }
        override public DateTime getDate() { return this.date; }
        override public int getBalance() { return this.balance; }
        public void setDescription(string edible) { this.depiction = edible; }
        public string getDescription() { return this.depiction; }
        override public string getNameClass() { return this.name_class; }

    }

    public class Overdue : Tovar
    {
        protected bool inedible;
        protected string name_class = "Учет просроченного товара";

        public Overdue(int id, bool isDeleted, byte type_record, string TovarName, DateTime date, int balance, bool inedible)
        {
            this.id = id;
            this.isDeleted = isDeleted;
            this.type_record = type_record;
            this.TovarName = TovarName;
            this.date = date;
            this.balance = balance;
            this.inedible = inedible;
        }

        override public void setTovar(string tovar) { this.TovarName = tovar; }
        override public void setDate(DateTime date) { this.date = date; }
        override public void setBalance(int balance) { this.balance = balance; }
        override public string getTovar() { return this.TovarName; }
        override public DateTime getDate() { return this.date; }
        override public int getBalance() { return this.balance; }
        public void setInedible(bool inedible) { this.inedible = inedible; }
        public bool getInedible() { return this.inedible; }
        override public string getNameClass() { return this.name_class; }
    }

    public interface IDataSource
    {
        /// <summary>
        /// Сохранение записи в хранилище.
        /// Если у записи id == 0, то значит выполняется добавление новой записи,
        /// для нее нужно сгенерировать id (порядковый номер). Иначе - обновление записи
        /// </summary>
        /// <param name="record">Добавляемая или обновляемая запись</param>
        /// <returns>Запись из хранилищая с id</returns>
        Tovar Save(Tovar record);
        /// <summary>
        /// Возвращает одну запись из хранилища по ее идентификатору
        /// </summary>
        /// <param name="id">Идентификатор записи</param>
        /// <returns>Найденная запись или null, если записи с таким id нет</returns>
        Tovar Get(int id);
        /// <summary>
        /// Удаляет одну запись из хранилища по ее идентификатору
        /// </summary>
        /// <param name="id">Идентификатор записи</param>
        /// <returns>true, если запись успешно удалена</returns>
        bool Delete(int id);
        /// <summary>
        /// Возвращает все записи из хранилища
        /// </summary>
        /// <returns>Все записи</returns>
        List<Tovar> GetAll();
    }

    public class FileDataSource : IDataSource
    {
        private List<Tovar> records = new List<Tovar>();
        string path = @"C:\Users\Кирилл\Desktop\Shklyaev_3Lab\Shklyaev_3Lab\record.dat";
        const string surname = "shklyaev";
        const string name = "kirill";
        const string ot = "alekseevich";
        // Конструктор
        public FileDataSource()
        {
            byte[] signature = calcucateSignature(surname, name, ot);
            if (!(File.Exists(path)))
            { // Создание файла
                using (BinaryWriter writer = new BinaryWriter(File.Open(path, FileMode.OpenOrCreate)))
                {
                    writer.Write(signature);
                }
            }
            else // Чтение файла
            {
                Tovar record;
                using (BinaryReader reader = new BinaryReader(File.Open(path, FileMode.Open)))
                {
                    for (int i = 0; i < 24; i++)
                    {
                        if (reader.ReadByte() != signature[i]) throw new Exception("Сигнатура ложная!");
                    }
                    while (reader.PeekChar() > -1)
                    {
                        bool isDeleted = reader.ReadBoolean();
                        byte type_record = reader.ReadByte();
                        int id = reader.ReadInt32();
                        string tovar = reader.ReadString();
                        int balance = reader.ReadInt32();
                        DateTime date = new DateTime(reader.ReadInt64());
                        if (type_record == 1)
                        {
                            string description = reader.ReadString();
                            records.Add(new Description(id, isDeleted, type_record, tovar, date, balance, description));
                        }
                        else
                        {
                            bool inedible = reader.ReadBoolean();
                            records.Add(new Overdue(id, isDeleted, type_record, tovar, date, balance, inedible));
                        }
                    }
                }
            }
        }
        private byte[] calcucateSignature(string surname, string name, string ot)
        {
            byte[] signature = new byte[24];
            for (int i = 0; i < 8; i++)
                if (surname.Length > i) signature[i] = (byte)surname[i];
            for (int i = 0; i < 8; i++)
                if (name.Length > i) signature[i + 8] = (byte)name[i];
            for (int i = 0; i < 8; i++)
                if (ot.Length > i) signature[i + 16] = (byte)ot[i];
                else { signature[i + 16] = 00000000; }
            return signature;
        }

        public Tovar Save(Tovar record)
        {
            if (record.id == 0)
            {
                record.id = records.Count + 1;
                this.records.Add(record); // Добавление записи
                using (BinaryWriter writer = new BinaryWriter(File.Open(path, FileMode.Append))) // Запись в файл
                {
                    writer.Write(record.isDeleted);
                    writer.Write(record.type_record);
                    writer.Write(record.id);
                    writer.Write(record.getTovar());
                    writer.Write(record.getBalance());
                    writer.Write(record.getDate().Ticks);
                    if (record.type_record == 1)
                        writer.Write(((Description)record).getDescription());
                    else writer.Write(((Overdue)record).getInedible());
                }
            }
            else
            {
                for (int i = 0; i < records.Count; i++)
                {
                    // Обновление записи
                    if (records[i].id == record.id)
                    {
                        records[i] = record;
                        return record;
                    }
                }
                throw new Exception("Такой записи не найдено");
            }
            return record;
        }

        public Tovar Get(int id)
        {
            foreach (Tovar record in records)
                if (record.id == id) return record;
            throw new Exception("Такого элемента нету");
        }
        public bool Delete(int id)
        {
            for (int i = 0; i < records.Count; i++)
            {
                if (records[i].id == id)
                {
                    records.RemoveAt(i);
                    updateFile();
                    return true;
                }
            }
            return false;
        }
        private void updateFile()
        {
            byte[] signature = calcucateSignature(surname, name, ot);
            using (BinaryWriter writer = new BinaryWriter(File.Open(path, FileMode.Create)))
            {
                writer.Write(signature);
                foreach (Tovar record in records)
                {
                    writer.Write(record.isDeleted);
                    writer.Write(record.type_record);
                    writer.Write(record.id);
                    writer.Write(record.getTovar());
                    writer.Write(record.getBalance());
                    writer.Write(record.getDate().Ticks);
                    if (record.type_record == 1)
                        writer.Write(((Description)record).getDescription());
                    else writer.Write(((Overdue)record).getInedible());
                }
            }
        }
        public List<Tovar> GetAll()
        {
            Tovar[] arr = records.ToArray();
            Array.Sort(arr);
            return new List<Tovar>(arr);
        }
    }

    // Бизнес-логика
    public class BusinessLogic
    {
        private IDataSource dataSource;

        public BusinessLogic(IDataSource source)
        {
            dataSource = source;
        }
        public List<Tovar> GetList()
        {
            List<Tovar> list = dataSource.GetAll();
            return list;
        }

        public Tovar Save(Tovar record)
        {
            dataSource.Save(record);
            return record;
        }

        public Tovar Get(int id)
        {
            return dataSource.Get(id);
        }

        public bool Delete(int id)
        {
            return dataSource.Delete(id);
        }
        public void generateReport(string path, DateTime date_start, DateTime date_end)
        {
            List<Tovar> all_records = dataSource.GetAll();
            List<Tovar> records = new List<Tovar>();
            foreach (Tovar ord in all_records)
                if (ord.getDate() > date_start && ord.getDate() < date_end && ord.getDate() > date_start && ord.getDate() < date_end) records.Add(ord);
            // Генерация отчета
            using (StreamWriter sw = new StreamWriter($"{path}", false, System.Text.Encoding.Default))
            {
                sw.WriteLine("Отчет: " + date_start + " - " + date_end);
                for (int i = 0; i < records.Count; i++)
                    sw.WriteLine("Клиент: " + records[i].getTovar() + ", Сумма ремонта: " + records[i].getBalance());
            }
        }
    }
    static class Program
    {
        public static BusinessLogic logic = new BusinessLogic(new FileDataSource());

        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}

    


using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LAB3
{
    static class Program {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main() {
            for (int i = 10; i < 150; i+= 10)
            {
                Random rnd = new Random(123);
                Console.WriteLine("New iteration. i = :{0}", i);
                Compute.Run(25, i, 10, 70, rnd);
                Console.WriteLine('\n');
            }
            // Compute.Run(25, 100, 10, 40);

        }
    }
    static class Compute {
        public static int counter = 0;
        static int amountOfTasks = 0;
        static long waitingTime = 0;
        static long sleep = 0;
        static long processingTimeSum = 0;
        static Random rnd;
        static List<Task> taskList = new List<Task>();
        public static List<long> waitTime = new List<long>();
        public static void Run(long intensity, int tasksAmount, int startOfRange, int endOfRange, Random rnd1) {
            Clean();
            rnd = rnd1;
            bool isProcessorBusy = false;
            while (amountOfTasks + 1 <= tasksAmount || isProcessorBusy || taskList.Count != 0) {

                if (amountOfTasks + 1 <= tasksAmount) {
                    taskList.Add(new Task(amountOfTasks, counter, rnd.Next(startOfRange, endOfRange)));
                    amountOfTasks++;
                }

                isProcessorBusy = Processor.CheckProcessor();
                if (taskList.Count > 0 && !isProcessorBusy) {
                    Processor.ComputeTask(taskList[taskList.Count - 1]);
                    // Console.WriteLine("Task Id = :{0}\t Waiting Time = :{1}\t Proccesing time = :{2}", taskList[^1].id, waitingTime, taskList[^1].ExecutionTime);
                    waitTime.Add(waitingTime);
                    processingTimeSum += taskList[^1].ExecutionTime;
                    sleep += waitingTime - intensity < 0 ? intensity - waitingTime : 0;
                    waitingTime = waitingTime + taskList[taskList.Count - 1].ExecutionTime - intensity < 0 ? 0 : waitingTime + taskList[taskList.Count - 1].ExecutionTime - intensity;
                    taskList.RemoveAt(taskList.Count - 1);
                }
                counter++;
            }
            long waitTimeSum = 0;
            foreach (int i in waitTime) {
                waitTimeSum += i;
            }

            float idle = (float)((float)sleep / (float)(waitTimeSum + sleep + processingTimeSum) * 100.0);
            Console.WriteLine("Avg waiting time = :{0}", waitTimeSum / tasksAmount);
            Console.WriteLine("Resource idle time in persent = :{0}%", idle);
        }
        private static void Clean() {
            amountOfTasks = 0;
            waitTime.Clear();
            waitingTime = 0;
            processingTimeSum = 0;
            sleep = 0;
            Processor.Clear();
        }
    }
    internal static class Processor {
        static Task t;
        static int taskWait = 0;
        public static int ProcessorFreeTime { get; internal set; }
        public static int ProcessorWorkTime { get; internal set; }
        static internal bool CheckProcessor()
        {
            taskWait--;
            if (taskWait <= 0)
            {
                if (taskWait < 0)
                {
                    ProcessorFreeTime++;
                }
                ProcessorWorkTime++;
                return false;
            }
            else
            {
                ProcessorWorkTime++;
                return true;
            }
        }
        static internal void ComputeTask(Task t)
        {
            Processor.t = t;
            taskWait = t.ExecutionTime;
        }

        static internal void Clear()
        {
            taskWait = 0;
        }
    }
    internal struct Task
    {
        public int id { get; private set; }
        public int CreationTime { get; private set; }
        public int ExecutionTime { get; private set; }
        public Task(int id, int creationTime, int executionTime)
        {
            this.id = id;
            this.CreationTime = creationTime;
            this.ExecutionTime = executionTime;
        }
    }
}
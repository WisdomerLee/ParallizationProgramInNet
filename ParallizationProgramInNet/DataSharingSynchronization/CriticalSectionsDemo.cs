using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParallizationProgramInNet.DataSharingSynchronization
{
    //Critical Section : 1개의 스레드만 접근할 수 있는 영역(접근을 제한하는 효과)
    #region CriticalSection
    class BankAccount
    {
        public object padlock = new object();
        public int Balance { get; private set; }
        public void Deposit(int amount)
        {
            //+=
            //op1: temp <- get_Balance()+ amount
            //op2:  set_Balance(temp)
            lock (padlock)
            {
                Balance += amount;
            }

        }
        public void Withdraw(int amount)
        {
            lock (padlock)
            {
                Balance -= amount;
            }
        }

    }

    #endregion
    #region Interlocked Operation
    class BankAccountInterlock
    {
        int balance;
        public int Balance
        {
            get => balance; private set
            {
                balance = value;
            }
        }
        public void Deposit(int amount)
        {
            //Interlocked : lock 방식을 자동으로 하도록 처리해주는 함수 중 하나 : 증가, 교체, 같으면 해당 값과 같으면 교체 등등의 함수들을 제공함 : 한 번에 하나씩 처리하도록 보장함
            Interlocked.Add(ref balance, amount);
        }
        public void Withdraw(int amount)
        {
            Interlocked.Add(ref balance, -amount);
        }
    }
    #endregion
    #region Spin Locking and Lock Recursion
    //기본적으로 locking은 CPU의 사이클을 낭비하게 되는 효과도 갖고 오므로.. CPU cycle의 낭비를 최소화 하기 위한 방식으로 spin locking, lock recursion 방식을 도입하게 됨
    class BankAccountSpinLock
    {
        int balance;
        public int Balance
        {
            get => balance; private set
            {
                balance = value;
            }
        }
        public void Deposit(int amount)
        {
            balance += amount;
        }
        public void Withdraw(int amount)
        {
            balance -= amount;
        }
    }
    #endregion
    #region Mutex
    class BankAccountMutex
    {
        int balance;
        public int Balance
        {
            get => balance; private set
            {
                balance = value;
            }
        }
        public void Deposit(int amount)
        {
            balance += amount;
        }
        public void Withdraw(int amount)
        {
            balance -= amount;
        }
        public void Transfer(BankAccountMutex where, int amount)
        {
            Balance -= amount;
            where.Balance += amount;
        }
    }
    #endregion
    /// <summary>
    /// MemoryBarrier
    ///1
    ///2
    ///Thread.MemoryBarrier();
    ///3 1,2 작업 이전엔 객체 생성 없이 하다 3번째 작업 이전에 memory barrier를 형성하여 3에서 처리할 때 하나씩 접근하도록 하는 방식...도 있음
    /// </summary>

    internal class CriticalSectionsDemo
    {
        SpinLock spinLockLR = new SpinLock(true);
        //ReaderWriterLock도 있으나 강의에서는 Slim 사용을 권장
        ReaderWriterLockSlim padLock = new ReaderWriterLockSlim();
        //아래와 같이 padLock을 설정해 두면 padLock에서 EnterReadLock을 여러번 호출(Recursion)하여도 Task에서 접근이 가능하게 바뀜... 그러나 추천하는 방식은 아님
        //ReaderWriterLockSlim padLock = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);
        Random random = new Random();
        void Main()
        {
            CriticalSection();
            InterLockedSection();
            SpinLockSection();

        }
        void CriticalSection()
        {
            var tasks = new List<Task>();
            var ba = new BankAccount();
            for (int i = 0; i < 10; i++)
            {
                tasks.Add(Task.Factory.StartNew(() =>
                {
                    for (int j = 0; j < 1000; j++)
                    {
                        ba.Deposit(100);
                    }
                }));
                tasks.Add(Task.Factory.StartNew(() =>
                {
                    for (int j = 0; j < 1000; j++)
                    {
                        ba.Withdraw(100);
                    }
                }));

            }
            Task.WaitAll(tasks.ToArray());
            Console.WriteLine($"Final balance is {ba.Balance}");
        }
        void InterLockedSection()
        {
            var tasks = new List<Task>();
            var ba = new BankAccountInterlock();
            for (int i = 0; i < 10; i++)
            {
                tasks.Add(Task.Factory.StartNew(() =>
                {
                    for (int j = 0; j < 1000; j++)
                    {
                        ba.Deposit(100);
                    }
                }));
                tasks.Add(Task.Factory.StartNew(() =>
                {
                    for (int j = 0; j < 1000; j++)
                    {
                        ba.Withdraw(100);
                    }
                }));

            }
            Task.WaitAll(tasks.ToArray());
            Console.WriteLine($"Final balance is {ba.Balance}");
        }
        void SpinLockSection()
        {
            var tasks = new List<Task>();
            var ba = new BankAccountSpinLock();

            SpinLock sl = new SpinLock();
            //Dead Lock상태가 나타날 수도 있음 (서로 다른 데이터를 점유하고 해당 데이터를 처리하기 위해 이미 다른 Task가 점유하고 있는 데이터에 접근하여 처리하기 위해 무한히 대기하게 되는 현상)
            for (int i = 0; i < 10; i++)
            {
                tasks.Add(Task.Factory.StartNew(() =>
                {
                    for (int j = 0; j < 1000; j++)
                    {
                        var lockTaken = false;
                        try
                        {
                            //Monitor.TryEnter()와 같은 효과로 들어가게 됨 : 상황을 보고 데이터에 접근할 수 있으면 접근하여 다른 Task가 접근하지 못하게 자물쇠를 걸어둠
                            sl.Enter(ref lockTaken);
                            ba.Deposit(100);
                        }
                        finally
                        {
                            if (lockTaken)
                            {
                                sl.Exit();
                            }
                        }
                    }
                }));
                tasks.Add(Task.Factory.StartNew(() =>
                {
                    for (int j = 0; j < 1000; j++)
                    {
                        var lockTaken = false;
                        try
                        {
                            //Monitor.TryEnter()와 같은 효과로 들어가게 됨 : 상황을 보고 데이터에 접근할 수 있으면 접근하여 다른 Task가 접근하지 못하게 자물쇠를 걸어둠
                            sl.Enter(ref lockTaken);
                            ba.Withdraw(100);
                        }
                        finally
                        {
                            if (lockTaken)
                            {
                                sl.Exit();
                            }
                        }
                    }
                }));

            }
            Task.WaitAll(tasks.ToArray());
            Console.WriteLine($"Final balance is {ba.Balance}");
        }
        //아래와 같이 코드를 짜게 되면 Exception 이 발생, SpinLock : LockRecursion을 지원하지 않음... Lock Recursion은 몹시 조심스럽고 위험한 경우가 많음... 예외 등등
        void LockRecursionSectionFail(int x)
        {

            bool lockTaken = false;

            try
            {
                spinLockLR.Enter(ref lockTaken);

            }
            catch (LockRecursionException ex)
            {
                Console.WriteLine($"Exception {ex}");
            }
            finally
            {
                if (lockTaken)
                {
                    Console.WriteLine($"Took a lock, x = {x}");
                    LockRecursionSectionFail(x - 1);
                    spinLockLR.Exit();
                }
                else
                {
                    Console.WriteLine($"Failed to take a lock, x = {x}");
                }
            }
        }

        //WaitHandle이벤트에서 Reset 이벤트 함수를 호출하는 형태로...
        void MutexSection()
        {
            var tasks = new List<Task>();
            var ba = new BankAccountMutex();
            var ba2 = new BankAccountMutex();
            Mutex mutex = new Mutex();
            Mutex mutex2 = new Mutex();
            for (int i = 0; i < 10; i++)
            {
                tasks.Add(Task.Factory.StartNew(() =>
                {
                    for (int j = 0; j < 1000; j++)
                    {
                        bool haveLock = mutex.WaitOne();
                        try
                        {
                            ba.Deposit(100);
                        }
                        finally
                        {
                            if (haveLock)
                            {
                                mutex.ReleaseMutex();
                            }
                        }
                    }
                }));
                tasks.Add(Task.Factory.StartNew(() =>
                {
                    for (int j = 0; j < 1000; j++)
                    {
                        bool haveLock = mutex.WaitOne();
                        try
                        {
                            ba.Withdraw(100);
                        }
                        finally
                        {
                            if (haveLock)
                            {
                                mutex.ReleaseMutex();
                            }
                        }
                    }
                }));

            }

            for (int i = 0; i < 10; i++)
            {
                tasks.Add(Task.Factory.StartNew(() =>
                {
                    for (int j = 0; j < 1000; j++)
                    {
                        bool haveLock = mutex.WaitOne();
                        try
                        {
                            ba.Deposit(1);
                        }
                        finally
                        {
                            if (haveLock)
                            {
                                mutex.ReleaseMutex();
                            }
                        }
                    }
                }));
                tasks.Add(Task.Factory.StartNew(() =>
                {
                    for (int j = 0; j < 1000; j++)
                    {
                        bool haveLock = mutex2.WaitOne();
                        try
                        {
                            ba2.Withdraw(1);
                        }
                        finally
                        {
                            if (haveLock)
                            {
                                mutex2.ReleaseMutex();
                            }
                        }
                    }
                }));
                tasks.Add(Task.Factory.StartNew(() =>
                {
                    for (int j = 0; j < 1000; j++)
                    {
                        bool haveLock = WaitHandle.WaitAll(new[] { mutex, mutex2 });
                        try
                        {
                            ba.Transfer(ba2, 1);
                        }
                        finally
                        {
                            if (haveLock)
                            {
                                mutex.ReleaseMutex();
                                mutex2.ReleaseMutex();
                            }
                        }
                    }
                }));
            }
            Task.WaitAll(tasks.ToArray());
            Console.WriteLine($"Final balance ba is {ba.Balance}");
            Console.WriteLine($"Final balance ba2 is {ba2.Balance}");
        }
        //Mutex : global 접근 가능 : 서로 다른 앱끼리도 공유 가능한 자원
        void MutexAdvancedSection()
        {
            string appName = "MyApp";
            Mutex mutex;
            try
            {
                mutex = Mutex.OpenExisting(appName);
                Console.WriteLine("Sorry, {appName} is already running");
            }
            catch (WaitHandleCannotBeOpenedException e)
            {
                Console.WriteLine("We can run the program first");
                mutex = new Mutex(false, appName);
            }
            Console.ReadKey();
            mutex.ReleaseMutex();
        }
        /// <summary>
        /// ReadLock, WriteLock사용 : 싱크 맞추기
        /// </summary>
        void ReaderWriterLockSection()
        {
            int x = 0;
            var tasks = new List<Task>();
            for (int i = 0; i < 10; i++)
            {
                tasks.Add(Task.Factory.StartNew(() =>
                {
                    padLock.EnterReadLock();

                    Console.WriteLine($"Entered read lock, x = {0}");
                    Thread.Sleep(1000);

                    padLock.ExitReadLock();
                    Console.WriteLine($"Exited read lock, x = {x}");
                }));
            }
            try
            {
                Task.WaitAll(tasks.ToArray());
            }
            catch(AggregateException e)
            {
                e.Handle(e =>
                {
                    Console.WriteLine(e);
                    return true;
                });
            }
            while (true)
            {
                Console.ReadKey();
                padLock.EnterWriteLock();
                Console.Write("Write lock acquired");
                int newValue = random.Next(10);
                x = newValue;
                Console.WriteLine($"Set x = {x}");
                padLock.ExitWriteLock();
                Console.WriteLine("Write lock released");
            }
        }
        /// <summary>
        /// ReadLock에 들어간 상태로 WriteLock 상태에 들어가게 되면 Dead Lock 상태가 호출됨.. 값을 읽고 특정 상황시 값을 변경해야 하는 경우가 생긴다면 저 경우에 쓸 수 없음..
        /// UpgradeableReadLock을 쓰면 WriteLock 상태로 바꿀 수 있게 됨..
        /// </summary>
        void UpgradeLockSection()
        {
            int x = 0;
            var tasks = new List<Task>();
            for (int i = 0; i < 10; i++)
            {
                tasks.Add(Task.Factory.StartNew(() =>
                {
                    padLock.EnterUpgradeableReadLock();

                    if (i % 2 == 0)
                    {
                        padLock.EnterWriteLock();
                        x = 123;
                        padLock.ExitWriteLock();
                    }
                    Console.WriteLine($"Entered read lock, x = {0}");
                    Thread.Sleep(1000);

                    padLock.ExitUpgradeableReadLock();
                    
                    Console.WriteLine($"Exited read lock, x = {x}");
                }));
            }
            try
            {
                Task.WaitAll(tasks.ToArray());
            }
            catch (AggregateException e)
            {
                e.Handle(e =>
                {
                    Console.WriteLine(e);
                    return true;
                });
            }
            while (true)
            {
                Console.ReadKey();
                padLock.EnterWriteLock();
                Console.Write("Write lock acquired");
                int newValue = random.Next(10);
                x = newValue;
                Console.WriteLine($"Set x = {x}");
                padLock.ExitWriteLock();
                Console.WriteLine("Write lock released");
            }
        }
    }
}

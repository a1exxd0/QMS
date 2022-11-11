using Newtonsoft.Json;
namespace QMS.QMSScripts
{
    #region Complex
    /// <summary>
    /// Simple class for complex numbers, used because System.Numerics.Complex
    /// is far too overloaded for what I want to use it for.
    /// </summary>
    [JsonObject]internal class Complex
    {
        [JsonProperty]
        internal float RP; //real part
        [JsonProperty]
        internal float IP; //imaginary part


        /// <summary>
        /// Constructor for complex numbers
        /// </summary>
        /// <param name="reals">Real part</param>
        /// <param name="imaginary">Imaginary part</param>
        [JsonConstructor] internal Complex(float reals, float imaginary)
        {
            RP = reals;
            IP = imaginary;
        }
        /// <summary>
        /// Calculates conjugate of number
        /// </summary>
        internal Complex Conj()
        {
            return new Complex(RP, -1 * IP);
        }
        /// <summary>
        /// Multiplies number by its conjugate to get square magnitude
        /// </summary>
        /// <returns>Return num*num conjugate</returns>
        internal float magnitudeSquared()
        {
            return (RP * RP + IP * IP); //(a+ib)(a-ib) = a^2 + b^2
        }
        /// <summary>
        /// Multiplies 2 complex numbers
        /// </summary>
        /// <param name="b">Number you want to multiply with</param>
        /// <returns>Returns result</returns>
        internal Complex Multiply(Complex b)
        {
            //(a+ib)(c+id) = ac-bd + (bc+ad)i
            return (new Complex((RP * b.RP) + (IP * b.IP * -1), (RP * b.IP + IP * b.RP)));
        }
        /// <summary>
        /// Adds 2 complex numbers
        /// </summary>
        /// <param name="b">Number to add</param>
        /// <returns>Returns result of addition</returns>
        internal Complex Add(Complex b)
        {
            return (new Complex(RP + b.RP, IP + b.IP));
        }
    }
    #endregion


    public class QubitModule
    {
        //Standard Gates
        internal static readonly Complex[,] I = new Complex[2, 2]
        {
            {new Complex(1, 0), new Complex(0, 0)},
            {new Complex(0, 0), new Complex(1, 0)},
        };
        internal static readonly Complex[,] H = new Complex[2, 2]
        {
            {new Complex(0.7071f, 0), new Complex(0.7071f, 0)},
            {new Complex(0.7071f, 0), new Complex(-0.7071f, 0)},
        };
        internal static readonly Complex[,] PauliX = new Complex[2, 2]
        {
            {new Complex(0, 0), new Complex(1, 0)},
            {new Complex(1, 0), new Complex(0, 0)},
        };
        internal static readonly Complex[,] PauliY = new Complex[2, 2]
        {
            {new Complex(0, 0), new Complex(0, -1)},
            {new Complex(0, 1), new Complex(0, 0)},
        };
        internal static readonly Complex[,] PauliZ = new Complex[2, 2]
        {
            {new Complex(1, 0), new Complex(0, 0)},
            {new Complex(0, 0), new Complex(-1, 0)},
        };
        internal static readonly Complex[,] CNOT = new Complex[4, 4]
        {
            {new Complex(1, 0), new Complex(0, 0),
            new Complex(0, 0), new Complex(0, 0)},

            {new Complex(0, 0), new Complex(1, 0),
            new Complex(0, 0), new Complex(0, 0)},

            {new Complex(0, 0), new Complex(0, 0),
            new Complex(0, 0), new Complex(1, 0)},

            {new Complex(0, 0), new Complex(0, 0),
            new Complex(1, 0), new Complex(0, 0)}
        };
        /// <summary>
        /// Takes in a vector in the form of a list
        ///For each entity in the vector, it will find the square magnitude of the
        ///entity and add it to a sum.
        ///
        ///The sum is the total square magnitude, and to find the magnitude alone,
        ///the normalization vector, it must be square rooted.
        ///The final result will be the normalization factor for the vector(as it 
        ///must be of 'magnitude' 1 as a mathematical law)
        /// </summary>
        /// <param name="listOfVectorEntities">
        /// List of numbers you want to find NF for</param>
        /// <returns>Returns a float in the form of a complex number, but its reciprocal</returns>
        static internal Complex findNormalizationFactor(List<Complex> listOfVectorEntities)
        {
            float sumOfSquaredProbabilities = 0;
            for (int i = 0; i < listOfVectorEntities.Count; i++)
            {
                sumOfSquaredProbabilities = sumOfSquaredProbabilities
                    + listOfVectorEntities[i].magnitudeSquared();
            }
            return (new Complex(1 / MathF.Sqrt(sumOfSquaredProbabilities), 0));
        }

        /// <summary>
        /// Takes in 2 differing arrays of size ixj, kxl
        /// Returns tensor product of size(ixk, jxl)
        /// </summary>
        /// <param name="A">Matrix 1</param>
        /// <param name="B">Matrix 2</param>
        /// <returns>Returns tensor product</returns>
        static internal Complex[,] tensorProduct(Complex[,] A, Complex[,] B)
        {
            Complex[,] answer = new Complex[A.GetLength(0) * B.GetLength(0), A.GetLength(1) * B.GetLength(1)];
            for (int i = 0; i < A.GetLength(0); i++) //each row in A
            {
                for (int j = 0; j < A.GetLength(1); j++) //each column in A
                {
                    Complex leftarg = A[i, j]; // therefore each element
                    for (int k = 0; k < B.GetLength(0); k++) //each row in B
                    {
                        for (int l = 0; l < B.GetLength(1); l++) //each column in B
                        {
                            answer[i * B.GetLength(0) + k, j * B.GetLength(1) + l]
                                //          ^row in answer          ^column
                                = leftarg.Multiply(B[k, l]);
                            // ^Product
                        }
                    }
                }
            }
            return (answer);
        }
        /// <summary>
        /// Multiplies 2 complex matricies.
        /// 
        /// WARNING: bad inputs are NOT detected.
        /// </summary>
        /// <param name="A">Matrix 1</param>
        /// <param name="B">Matrix 2</param>
        /// <returns>Returns product</returns>
        static internal Complex[,] matrixProduct(Complex[,] A, Complex[,] B)
        {
            Complex temp = new Complex(0, 0);
            Complex[,] result = new Complex[A.GetLength(0), B.GetLength(1)];

            for (int i = 0; i < A.GetLength(0); i++)
            {
                for (int j = 0; j < B.GetLength(1); j++)
                {
                    temp = new Complex(0, 0);
                    for (int k = 0; k < A.GetLength(1); k++)
                    {
                        temp = temp.Add(A[i, k].Multiply(B[k, j]));
                    }
                    result[i, j] = temp;
                }
            }
            return result;
        }
        /// <summary>
        /// Used for the 'middleman': the 2 middle qubits in an
        /// entanglement swap are measured and the measurement correction
        /// is applied to the recieving entangled qubit 4
        ///
        /// Takes in measurement RESULTS and returns the appropriate correction
        /// to make sure entangled states are the same orientation, making
        /// it easier for me to code the messaging system.
        /// </summary>
        /// <param name="Qubit1">RESULT for Qubit 1</param>
        /// <param name="Qubit2">RESULT for Qubit 2</param>
        /// <returns>Gate needed to return system to good orientation</returns>
        internal static Complex[,] bellStateAnalyser(int Qubit1, int Qubit2)
        {
            if (Qubit1 == 0)
            {
                if (Qubit2 == 0) { return I; }
                else { return PauliX; }
            }
            else
            {
                if (Qubit2 == 0) { return PauliZ; }
                else { return matrixProduct(PauliZ, PauliX); }
            }
        }

        /// <summary>
        /// Troubleshooting usage function for matrix printing
        /// </summary>
        /// <param name="A"></param>
        internal static void Print2DComplex(Complex[,] A)
        {
            for (int i = 0; i < A.GetLength(0); i++)
            {
                for (int j = 0; j < A.GetLength(1); j++)
                {
                    Console.Write(A[i, j].RP + "+" + A[i, j].IP + "i, ");
                }
                Console.WriteLine();
            }
        }
    }
    /// <summary>
    /// Creates qubit system and allows for basic interactions through
    /// standard quantum system modelling.
    ///
    /// Allows for troubleshooting through being able to see state
    /// vector unlike other quantum simulaton modules.Also compact and fast.
    /// </summary>
    [JsonObject] public class QubitSystem
    {
        [JsonProperty]
        internal int numberOfQubits;
        [JsonProperty]
        internal int vectorSize;
        [JsonProperty]
        private Complex[,] systemVector; //nullable fields for error-free instantiation
        /// <summary>
        /// Integer power function
        /// </summary>
        /// <param name="x">Base</param>
        /// <param name="pow">POSITIVE exponent only</param>
        /// <returns></returns>
        public static int intPow(int x, int pow)
        {
            int ret = 1;
            while (pow != 0)
            {
                if ((pow & 1) == 1)
                    ret *= x;
                x *= x;
                pow >>= 1;
            }
            return ret;
        }
        /// <summary>
        /// Constructor for creating a system of qubits
        /// </summary>
        /// <param name="numberOfQubits">How many qubits</param>
        public QubitSystem(int numberOfQubits)
        {
            this.numberOfQubits = numberOfQubits; //how many qubits in system
            vectorSize = intPow(2, numberOfQubits); //how big the state vector is
            systemVector = new Complex[vectorSize, 1]; //create vector of suitable size
            systemVector[0, 0] = new Complex(1, 0); //set 1st element to 1
            for (int i = 1; i < vectorSize; i++)
            {
                systemVector[i, 0] = new Complex(0, 0); //set everything else to 0
            }
        }
        /// <summary>
        /// Returns encapsulated vector
        /// </summary>
        /// <returns>Value only, not reference</returns>
        internal Complex[,] getSystemVector()
        {
            return systemVector;
        }
        /// <summary>
        /// Adds to dictionary all non-zero probabilities and their
        /// corresponding state as the key.
        /// </summary>
        /// <returns>Dictionary(state, probability)</returns>
        internal Dictionary<string, Complex> getDiracVector()
        {
            Dictionary<string, Complex> diracVector = new Dictionary<string, Complex>();
            Complex tempCoefficient = new Complex(0, 0);
            string tempComponent;

            for (int i = 0; i < vectorSize; i++)
            {
                if ((systemVector[i, 0].RP != 0) || (systemVector[i, 0].IP != 0))
                {
                    tempCoefficient = systemVector[i, 0];
                    tempComponent = Convert.ToString(i, 2).PadLeft(numberOfQubits, '0');
                    //i = state value, base 2
                    //max length number of qubits, if it doesnt reach this, all leftmost are 0
                    diracVector.Add(tempComponent, tempCoefficient);
                }
            }

            return diracVector;
        }
        /// <summary>
        /// troubleshooting functionality
        /// </summary>
        internal static void PrintDiracVector(Dictionary<string, Complex> diracVector)
        {
            string temp;
            for (int i = 0; i < diracVector.Count; i++)
            {
                temp = diracVector.ElementAt(i).Key + " : " + diracVector.ElementAt(i).Value.RP
                    + " " + diracVector.ElementAt(i).Value.IP + "i";
                Console.WriteLine(temp);
            }
        }
        /// <summary>
        /// Applies 'Gate' to a single qubit
        /// Starts count at leftmost(highest value) qubit at 1
        /// </summary>
        /// <param name="targetQubit">Qubit you want to apply gate to</param>
        /// <param name="Gate">What SINGLE QUBIT gate(2x2) you want to use</param>
        internal void applyGateToOneQubit(int targetQubit, Complex[,] Gate)
        {
            int lengthToEnd = numberOfQubits - targetQubit;
            int lengthFromStart = targetQubit - 1;
            Complex[,] finalMatrix = Gate;

            for (int i = 0; i < lengthFromStart; i++)
            {
                finalMatrix = QubitModule.tensorProduct(QubitModule.I, finalMatrix);
            }
            for (int j = 0; j < lengthToEnd; j++)
            {
                finalMatrix = QubitModule.tensorProduct(finalMatrix, QubitModule.I);
            }
            systemVector = QubitModule.matrixProduct(finalMatrix, systemVector);
        }
        /// <summary>
        /// The parameter targetQubit is the control bit for this gate.
        /// 
        /// The next qubit along is the target qubit, but it is a 2 qubit gate so requires that
        /// the input is atleast 1 less than the number of qubits in the system.
        /// </summary>
        /// <param name="targetQubit">Control bit for gate; applies 'downwards'</param>
        internal void applyCNOT(int targetQubit)
        {
            int lengthToEnd = numberOfQubits - targetQubit - 1;
            int lengthFromStart = targetQubit - 1;
            Complex[,] finalMatrix = QubitModule.CNOT;

            for (int i = 0; i < lengthFromStart; i++)
            {
                finalMatrix = QubitModule.tensorProduct(QubitModule.I, finalMatrix);
            }
            for (int j = 0; j < lengthToEnd; j++)
            {
                finalMatrix = QubitModule.tensorProduct(finalMatrix, QubitModule.I);
            }
            systemVector = QubitModule.matrixProduct(finalMatrix, systemVector);
        }
        /// <summary>
        /// In an n-qubit system, it will set all relevant targetQubit probabilities
        /// that it will collapse into a zero as zero.
        /// 
        /// For example, for a 2 qubit system with state vector[0.5, 0.5, 0.5, 0.5], using removeZeroes(1)
        /// will result in [0, 0, 0.5, 0.5], and removeZeroes(2) will result in [0, 0.5, 0, 0.5]
        ///
        /// The targetQubit parameter counts starting at 1 for the leftmost qubit(highest binary value)
        /// </summary>
        /// <param name="targetQubit">Bit to collapse to 0 probability</param>
        internal void removeZeroes(int targetQubit)
        {
            int lengthToEnd = numberOfQubits - targetQubit;
            int iterationCount = intPow(2, lengthToEnd); //consecutive probabilities belonging
                                                         //to the same qubit
            int i = 0;
            while (i < vectorSize)
            {
                for (int j = 0; j < iterationCount; j++)
                {
                    systemVector[i + j, 0] = new Complex(0, 0);
                }
                i = i + iterationCount * 2; //skip past irrelevant probabilities
                if (i >= vectorSize)
                {
                    break;
                }
            }
        }
        /// <summary>
        /// In an n-qubit system, it will set all relevant targetQubit probabilities
        /// that it will collapse into a zero as zero.
        /// 
        /// For example, for a 2 qubit system with state vector[0.5, 0.5, 0.5, 0.5], using removeOnes(1)
        /// will result in [0.5, 0.5, 0, 0], and removeZeroes(2) will result in [0.5, 0, 0.5, 0]
        ///
        /// The targetQubit parameter counts starting at 1 for the leftmost qubit(highest binary value)
        /// </summary>
        /// <param name="targetQubit">Bit to collapse to 0 probability</param>
        internal void removeOnes(int targetQubit)
        {
            int lengthToEnd = numberOfQubits - targetQubit;
            int iterationCount = intPow(2, lengthToEnd);
            int i = iterationCount;
            while (i < vectorSize)
            {
                for (int j = 0; j < iterationCount; j++)
                {
                    systemVector[i + j, 0] = new Complex(0, 0);
                }
                i = i + iterationCount * 2;
                if (i >= vectorSize)
                {
                    break;
                }
            }
        }
        /// <summary>
        /// Only depends on the number of qubits in the system; each element in the vector
        /// represents a different possible combination of measured bits.
        ///
        /// Parameter targetQubit is for selecting which specific qubit you want to focus on
        /// Parameter state is for whether you want the targetQubit as a 1 or 0 to be selected
        ///
        /// Returns list of indexes relevant to parameters. For example in a 2 qubit system (length 4)
        /// with target qubit 1 and target state 1, the return will be(2,3), and in a 3 qubit system
        /// with target qubit 2 and target state 1 the return will be(2,3,6,7)
        /// </summary>
        /// <param name="targetQubit">Qubit you want to find states for</param>
        /// <param name="state">Whether you want 0's or 1's</param>
        /// <returns></returns>
        internal List<int> getStateIndexes(int targetQubit, int state)
        {
            List<int> stateIndexes = new List<int>();
            int LengthToEnd = numberOfQubits - targetQubit;
            int iterationCount = intPow(2, LengthToEnd);

            if (state == 0) //similar to removeZeroes/Ones algorithm
            {
                int i = 0;
                while (i < vectorSize)
                {
                    for (int j = 0; j < iterationCount; j++)
                    {
                        stateIndexes.Add(i + j);
                    }
                    i = i + iterationCount * 2;
                    if (i >= vectorSize)
                    {
                        break;
                    }
                }
            }
            else
            {
                int i = iterationCount;
                while (i < vectorSize)
                {
                    for (int j = 0; j < iterationCount; j++)
                    {
                        stateIndexes.Add(i + j);
                    }
                    i = i + iterationCount * 2;
                    if (i >= vectorSize)
                    {
                        break;
                    }
                }
            }

            return stateIndexes;
        }
        /// <summary>
        /// Takes in targetQubit as input and returns measurement value
        /// 
        /// Renormalizes state vector after for further manipulation
        /// </summary>
        /// <param name="targetQubit">Qubit you want measured</param>
        /// <returns>measurement output</returns>
        internal int measurement(int targetQubit)
        {
            List<Complex> listOfRelevantStatesForNormalization = new List<Complex>();
            int measurementOutput = selectNumber(targetQubit);

            List<int> relevantStateIndexes = getStateIndexes(targetQubit, measurementOutput);
            for (int i = 0; i < relevantStateIndexes.Count; i++)
            {
                listOfRelevantStatesForNormalization.Add(systemVector[relevantStateIndexes[i], 0]);
            }
            Complex normalizationFactor = QubitModule.findNormalizationFactor
                (listOfRelevantStatesForNormalization);

            if (measurementOutput == 0)
            {
                removeOnes(targetQubit);
                //if the bit is a guaranteed zero, then remove all probabilities of it being a 1
            }
            else { removeZeroes(targetQubit); } //vice-versa

            for (int j = 0; j < vectorSize; j++)
            {
                systemVector[j, 0] = systemVector[j, 0].Multiply(normalizationFactor);
                //renormalization of all elements
            }
            return measurementOutput;
        }
        /// <summary>
        /// takes in input targetQubit and performs a measurement based on
        /// probabilities from systemVector
        /// 
        /// Does not adjust the systemVector, and so should not be accessible outside the class
        /// </summary>
        /// <param name="targetQubit">Qubit you want to make a measurement of</param>
        /// <returns>Returns int 0, 1 as measured</returns>
        private int selectNumber(int targetQubit)
        {
            Random random = new Random();
            float selectedFloat = (float)random.NextDouble(); //select float between 0, 1

            List<Complex> listOfValidStates = new List<Complex>();
            List<int> placeOfValidStates = new List<int>();
            int lengthToEnd = numberOfQubits - targetQubit;
            int iterationCount = intPow(2, lengthToEnd);
            int i = 0;

            while (i < vectorSize) //for all relevant elements append to lists
            {
                for (int j = 0; j < iterationCount; j++)
                {
                    listOfValidStates.Add(systemVector[i + j, 0]);
                    placeOfValidStates.Add(i + j);
                }
                i = i + iterationCount * 2;
                if (i >= vectorSize)
                {
                    break;
                }
            }
            float sumOfZeroes = 0;
            for (int k = 0; k < listOfValidStates.Count; k++)
            {
                sumOfZeroes = sumOfZeroes + listOfValidStates.ElementAt(k).magnitudeSquared();
                //sum together everything relevant
            }

            List<Complex> listOfValidStates2 = new List<Complex>();
            List<int> placeOfValidStates2 = new List<int>();
            int l = iterationCount;
            while (l < vectorSize) //for all relevant probabilities for a 1
            {
                for (int m = 0; m < iterationCount; m++)
                {
                    listOfValidStates2.Add(systemVector[l + m, 0]);
                    placeOfValidStates2.Add(l + m);
                }
                l = l + iterationCount * 2; //skipping over irrelevant numbers
                if (l >= vectorSize)
                {
                    break;
                }
            }
            float sumOfOnes = 0;
            for (int n = 0; n < listOfValidStates.Count; n++)
            {
                sumOfOnes = sumOfOnes + listOfValidStates2.ElementAt(n).magnitudeSquared();
                //sum together everything relevant
            }

            float sumOfProbabilities = sumOfZeroes + sumOfOnes;
            float multiplier = 1 / sumOfProbabilities;
            //we want a+b = 1, so 1/a+b = x
            //and therefore ax+bx = 1

            if (selectedFloat <= multiplier * sumOfZeroes)
            {
                //if the random number is less than ax, a is selected
                return 0;
            }
            else { return 1; } //else 1 is selected

        }
        /// <summary>
        /// More list-printing troubleshooting functionality
        /// </summary>
        /// <param name="A"></param>
        internal static void PrintIntList(List<int> A)
        {
            for (int i = 0; i < A.Count; i++)
            {
                Console.Write(A[i] + ", ");
            }
        }
    }

    public class Program
    {
        /*
        static void Main()
        {
            QubitSystem EntSwap = new QubitSystem(8);
            EntSwap.applyGateToOneQubit(1, QubitModule.H);
            EntSwap.applyGateToOneQubit(3, QubitModule.H);

            EntSwap.applyCNOT(1);
            EntSwap.applyCNOT(3);
            EntSwap.applyCNOT(2);
            EntSwap.applyGateToOneQubit(2, QubitModule.H);
            EntSwap.applyGateToOneQubit(4, QubitModule.bellStateAnalyser
                (EntSwap.measurement(2), EntSwap.measurement(3)));

            EntSwap.applyGateToOneQubit(5, QubitModule.H);
            EntSwap.applyGateToOneQubit(7, QubitModule.H);

            EntSwap.applyCNOT(5);
            EntSwap.applyCNOT(7);
            EntSwap.applyCNOT(6);
            EntSwap.applyGateToOneQubit(6, QubitModule.H);
            EntSwap.applyGateToOneQubit(8, QubitModule.bellStateAnalyser
                (EntSwap.measurement(6), EntSwap.measurement(7)));

            QubitSystem.PrintDiracVector(EntSwap.getDiracVector());
        }
        */
    }
}

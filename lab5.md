*Паршин Максим, 343*

## Лабораторная работа #5

### Вариант 10

---

### Постановка задачи

Найти решение задачи 

$$
\begin{aligned}
&\frac{\partial^{2} u}{\partial t^{2}}=L u+f(x, t), \quad 0<x<1, \quad 0<t \leqslant 1, \\
&u(x, 0)=\varphi(x),\left.\quad \frac{\partial u}{\partial t}\right|_{t=0}=\psi(x), \quad 0 \leqslant x \leqslant 1, \\
&\left.\left(\alpha_{1}(t) u-\alpha_{2}(t) \frac{\partial u}{\partial x}\right)\right|_{x=0}=\alpha(t), 0 \leqslant t \leqslant 1, \\
&\left.\left(\beta_{1}(t) u+\beta_{2}(t) \frac{\partial u}{\partial x}\right)\right|_{x=1}=\beta(t), 0 \leqslant t \leqslant 1
\end{aligned}
$$

методом сеток, используя различные разностные схемы

### Исходные данные

$$
\begin{array}
\frac{\partial^{2} u}{\partial t^{2}}=\frac{\partial^{2} u}{\partial x^{2}}+\sin x \frac{\partial u}{\partial x}+f(x, t) \\
u(x, 0)=\varphi(x),\left.\quad \frac{\partial u}{\partial t}\right|_{t=0}=\psi(x), \quad 0 \leqslant x \leqslant 1 \\
u(0, t)-\left.\frac{\partial u}{\partial x}\right|_{x=0}=\alpha(t), \quad u(1, t)=\beta(t), 0 \leqslant t \leqslant 1
\end{array}
$$

Решения, на которых тестировалась задача

$$
\begin{array}
&u = x + t\\
u = x^3 + t^3
\end{array}
$$

Значения параметра $\sigma$ : $0,\ 1$
### **Код**

Github: https://github.com/mxprshn/NumMethods/blob/afdaf01a04a1e68a70ebf5f2a43573ed128297ac/numan/src/main/kotlin/laba5/lab5.kt

```kotlin
/**
 * Класс-представление точного решения системы с производными для тестирования
 */
data class ExactSolution(
    val u: (x : Double, t : Double) -> Double,
    val dudt: (x : Double, t : Double) -> Double,
    val dudx: (x : Double, t : Double) -> Double,
    val dudx2: (x : Double, t : Double) -> Double,
    val comment: String
)

/**
 * Точные решения для тестирования
 */
val exactSolutions = listOf(
    ExactSolution(
        u = { x: Double, t: Double -> x + t },
        dudt = { x: Double, t: Double -> 1.0 },
        dudx = { x: Double, t: Double -> 1.0 },
        dudx2 = { x: Double, t: Double -> 0.0 },
        "x + t"
    ),
    ExactSolution(
        u = { x: Double, t: Double -> x.pow(3) + t.pow(3) },
        dudt = { x: Double, t: Double -> 3 * t.pow(2) },
        dudx = { x: Double, t: Double -> 3 * x.pow(2) },
        dudx2 = { x: Double, t: Double -> 6 * x },
        "x^3 + t^3"
    )
)

val sigmas = listOf(0.0, 0.5, 1.0)

/**
 * Параметры системы
 */
val a = { x: Double, t: Double -> 1.0 }
val b = { x: Double, t: Double -> sin(x) }
val c = { x: Double, t: Double -> 0.0 }
val alpha1 =  { t: Double -> 1.0 }
val alpha2 =  { t: Double -> 0.0 }
val beta1 =  { t: Double -> 1.0 }
val beta2 =  { t: Double -> 1.0 }
val xFrom = 0.0
val xTo = 1.0
val tFrom = 0.0
val tTo = 0.1

fun main() {
    exactSolutions.forEach { solution ->
        println("EXACT SOLUTION: ${solution.comment}")
        sigmas.forEach { sigma ->
            println("SIGMA: $sigma")
            calculate(sigma, solution)
        }
    }
}

/**
 * Итерация с выводом результатов для конкретного точного решения u и значения sigma
 */
fun calculate(sigma: Double, exactSolution: ExactSolution) {
    val phi = { x: Double -> exactSolution.u(x, 0.0) }
    val alpha = { t: Double -> alpha1(t) * exactSolution.u(0.0, t) - alpha2(t) * exactSolution.dudx(0.0, t) }
    val beta = { t: Double -> beta1(t) * exactSolution.u(1.0, t) + beta2(t) * exactSolution.dudx(1.0, t) }
    val f = { x: Double, t: Double -> exactSolution.dudt(x, t) - a(x, t) * exactSolution.dudx2(x, t) - b(x, t) * exactSolution.dudx(x, t) - c(x, t) * exactSolution.u(x, t) }
    val equation = ParabolicEquation(a, b, c, f, alpha, alpha1, alpha2, beta, beta1, beta2, phi, xFrom, xTo, tFrom, tTo)

    fun printSolution(solution: RealMatrix, N: Int, M: Int, h: Double, tau: Double) {
        table {
            cellStyle {
                border = true
                alignment = TextAlignment.TopCenter
            }
            header {
                row {
                    cell("")
                    (0..5).forEach { cell((xFrom + it * h).format()) }
                }

                (0..5).forEach { i ->
                    row {
                        cell((tFrom + i * tau).format())
                        (0..5).forEach { j -> cell(solution[i, j].format()) }
                    }
                }
            }
        }.also { println(it) }
    }

    listOf(5, 10, 20).forEach { N ->
        val M = N
        val solution = solveEquation(equation, sigma, N, M)
        val h = (equation.xTo - equation.xFrom) / N
        val tau = (equation.tTo - equation.tFrom) / M
        printSolution(solution, N, M, h, tau)
    }

    data class SolutionTableEntry(val h: Double, val tau: Double, val diffWithExact: Double, val diffWithPrev: Double?, val u: RealMatrix)
    val solutions = mutableListOf<SolutionTableEntry>()

    listOf(0.2, 0.1, 0.05).forEach { h ->
        val N = ((equation.xTo - equation.xFrom) / h).roundToInt()
        val tau = 0.01
        val M = ((equation.tTo - equation.tFrom) / tau).roundToInt()
        val exactSolutionTable = Array(1 + M) { DoubleArray(1 + N) { 0.0 } }.toMatrix()
        for (i in 0..M) {
            for (j in 0..N) {
                exactSolutionTable[i, j] = exactSolution.u(xFrom + j * h, tFrom + i * tau)
            }
        }

        val foundSolution = solveEquation(equation, sigma, N, M)
        val diffWithExact = (exactSolutionTable.minus(foundSolution)).norm5()
        val diffWithPrev = if (solutions.isEmpty()) null else {
            solutions.last().u.minus(foundSolution.halfSubmatrix()).norm5()
        }

        solutions.add(
            SolutionTableEntry(h, tau, diffWithExact, diffWithPrev, foundSolution)
        )
    }

    table {
        cellStyle {
            border = true
            alignment = TextAlignment.TopCenter
        }
        header { row("h", "tau", "diff with exact", "diff with prev") }
        solutions.forEach {
            row(it.h.format(), it.tau.format(), it.diffWithExact.eFormat(), it.diffWithPrev?.eFormat())
        }
    }.also { println(it) }
}

/**
 * Решаем уравнение: поиск трехдиагональной матрицы и решение системы для каждого слоя
 */
fun solveEquation(equation: ParabolicEquation,
                  sigma: Double,
                  N: Int,
                  M: Int): RealMatrix {
    val h = (equation.xTo - equation.xFrom) / N
    val firstLayer = (0..N).map { equation.phi(equation.xFrom + it * h) }.toDoubleArray()
    val layers = mutableListOf(firstLayer)
    for (i in 1..M) {
        val prevLayer = layers[i - 1]
        val system = findTridiagonalSystemForLayer(equation, sigma, N, M, i, prevLayer)
        val currentLayer = solveTridiagonalSystem(system)
        layers.add(currentLayer)
    }
    return layers.toTypedArray().toMatrix()
}

/**
 * Класс-представление уравнения параболического типа с начальными и граничными условиями
 */
data class ParabolicEquation(
    val a: (x : Double, t : Double) -> Double,
    val b: (x : Double, t : Double) -> Double,
    val c: (x : Double, t : Double) -> Double,
    val f: (x : Double, t : Double) -> Double,
    val alpha: (t : Double) -> Double,
    val alpha1: (t : Double) -> Double,
    val alpha2: (t : Double) -> Double,
    val beta: (t : Double) -> Double,
    val beta1: (t : Double) -> Double,
    val beta2: (t : Double) -> Double,
    val phi: (x : Double) -> Double,
    val xFrom: Double,
    val xTo: Double,
    val tFrom: Double,
    val tTo: Double
)

/**
 * Ищем трехдиагональную матрицу системы для отдельного слоя по предыдущему слою
 */
fun findTridiagonalSystemForLayer(equation: ParabolicEquation,
                                  sigma: Double,
                                  N: Int,
                                  M: Int,
                                  k: Int,
                                  prevLayer: DoubleArray
): TridiagonalSystem {
    val h = (equation.xTo - equation.xFrom) / N
    val tau = (equation.tTo - equation.tFrom) / M

    val rows = mutableListOf<TridiagonalSystem.Row>()

    val tK = equation.tFrom + k * tau

    val Lu = { i: Int, xI: Double ->
        equation.a(xI, tK) * (prevLayer[i + 1] - 2.0 * prevLayer[i] + prevLayer[i - 1]) / (h.pow(2)) +
            equation.b(xI, tK) * (prevLayer[i + 1] - prevLayer[i - 1]) / (2.0 * h) + equation.c(xI, tK) * prevLayer[i]
    }

    for (i in 1 until N) {
        val xI = equation.xFrom + i * h
        rows.add(
            TridiagonalSystem.Row(
                A = sigma * (equation.a(xI, tK) / (h.pow(2)) - equation.b(xI, tK) / (2.0 * h)),
                B = sigma * (equation.a(xI, tK) * 2.0 / (h.pow(2)) - equation.c(xI, tK)) + 1.0 / tau,
                C = sigma * (equation.a(xI, tK) / (h.pow(2)) + equation.b(xI, tK) / (2.0 * h)),
                G = -prevLayer[i] / tau - (1.0 - sigma) * Lu(i, xI) - equation.f(xI, tK)
            )
        )
    }

    return TridiagonalSystem(
        B0 = -equation.alpha1(tK) - equation.alpha2(tK) / h,
        C0 = -equation.alpha2(tK) / h,
        G0 = equation.alpha(tK),
        AN1 = -equation.beta2(tK) / h,
        BN1 = -equation.beta1(tK) - equation.beta2(tK) / h,
        GN1 = equation.beta(tK),
        rows
    )
}

/**
 * Класс-представление системы с трехдиагональной матрицей
 */
data class TridiagonalSystem(
    val B0: Double,
    val C0: Double,
    val G0: Double,
    val AN1: Double,
    val BN1: Double,
    val GN1: Double,
    val rows: List<Row>
) {
    data class Row(
        val A: Double,
        val B: Double,
        val C: Double,
        val G: Double
    )
}

data class TridiagonalSolutionCoefficients(val s: Double, val t: Double)

/**
 * Решение системы с трехдиагональной матрицей методом прогонки
 */
fun solveTridiagonalSystem(system: TridiagonalSystem): DoubleArray {
    val coefficients = mutableListOf(
        TridiagonalSolutionCoefficients(
            s = system.C0 / system.B0,
            t = -system.G0 / system.B0
        )
    )

    system.rows.forEachIndexed { index, row ->
        val denominator = row.B - row.A * coefficients[index].s
        coefficients.add(
            TridiagonalSolutionCoefficients(
                s = row.C / denominator,
                t = (row.A * coefficients[index].t - row.G) / denominator
            )
        )
    }

    coefficients.add(
        TridiagonalSolutionCoefficients(
            s = 0.0,
            t = (system.AN1 * coefficients.last().t - system.GN1) /
                    (system.BN1 - system.AN1 * coefficients.last().s)
        )
    )

    val result = DoubleArray(system.rows.size + 2)
    result[result.size - 1] = coefficients.last().t
    for (i in result.size - 2 downTo 0) {
        result[i] = coefficients[i].s * result[i + 1] + coefficients[i].t
    }

    return result
}

/**
 * Из матрицы, соотвествующей более точной сетке с 2h, извлекаем матрицу,
 * соотвествующую сетке с h
 */
fun RealMatrix.halfSubmatrix(): RealMatrix {
    require(columnDimension % 2 == 1)

    val result = Array(rowDimension) { DoubleArray(columnDimension / 2 + 1) { 0.0 } }
    for (i in 0 until rowDimension) {

        for (j in 0 until columnDimension) {
            if (j % 2 != 0) continue
            result[i][j / 2] = this[i, j]
        }
    }

    return result.toMatrix()
}

/**
 * Норма матрицы по подматрице 6x6
 */
fun RealMatrix.norm5(): Double {
    require(rowDimension > 5)
    require(columnDimension > 5)

    val submatrix = getSubMatrix(0, 5, 0, 5)
    return submatrix.norm
}
```

### Результаты

#### $u = x + t$

```
sigma = 0
┌─────────┬─────────┬─────────┬─────────┬─────────┬─────────┬─────────┐
│   t/x   │0.0000000│0.2000000│0.4000000│0.6000000│0.8000000│1.0000000│
├─────────┼─────────┼─────────┼─────────┼─────────┼─────────┼─────────┤
│0.0000000│0.0000000│0.2000000│0.4000000│0.6000000│0.8000000│1.0000000│
├─────────┼─────────┼─────────┼─────────┼─────────┼─────────┼─────────┤
│0.0200000│0.0200000│0.2200000│0.4200000│0.6200000│0.8200000│1.0200000│
├─────────┼─────────┼─────────┼─────────┼─────────┼─────────┼─────────┤
│0.0400000│0.0400000│0.2400000│0.4400000│0.6400000│0.8400000│1.0400000│
├─────────┼─────────┼─────────┼─────────┼─────────┼─────────┼─────────┤
│0.0600000│0.0600000│0.2600000│0.4600000│0.6600000│0.8600000│1.0600000│
├─────────┼─────────┼─────────┼─────────┼─────────┼─────────┼─────────┤
│0.0800000│0.0800000│0.2800000│0.4800000│0.6800000│0.8800000│1.0800000│
├─────────┼─────────┼─────────┼─────────┼─────────┼─────────┼─────────┤
│0.1000000│0.1000000│0.3000000│0.5000000│0.7000000│0.9000000│1.1000000│
└─────────┴─────────┴─────────┴─────────┴─────────┴─────────┴─────────┘
┌─────────┬─────────┬───────────────┬──────────────┐
│    h    │   tau   │diff with exact│diff with prev│
├─────────┼─────────┼───────────────┼──────────────┤
│0.2000000│0.0100000│ 4.4408921e-16 │              │
├─────────┼─────────┼───────────────┼──────────────┤
│0.1000000│0.0100000│ 2.1926905e-15 │5.8841820e-15 │
├─────────┼─────────┼───────────────┼──────────────┤
│0.0500000│0.0100000│ 4.6615489e-12 │4.9228954e-12 │
├─────────┼─────────┼───────────────┼──────────────┤
│0.0250000│0.0100000│ 2.9758641e-09 │3.2243216e-09 │
└─────────┴─────────┴───────────────┴──────────────┘
sigma = 1
┌─────────┬─────────┬─────────┬─────────┬─────────┬─────────┬─────────┐
│         │0.0000000│0.2000000│0.4000000│0.6000000│0.8000000│1.0000000│
├─────────┼─────────┼─────────┼─────────┼─────────┼─────────┼─────────┤
│0.0000000│0.0000000│0.2000000│0.4000000│0.6000000│0.8000000│1.0000000│
├─────────┼─────────┼─────────┼─────────┼─────────┼─────────┼─────────┤
│0.0200000│0.0200000│0.2200000│0.4200000│0.6200000│0.8200000│1.0200000│
├─────────┼─────────┼─────────┼─────────┼─────────┼─────────┼─────────┤
│0.0400000│0.0400000│0.2400000│0.4400000│0.6400000│0.8400000│1.0400000│
├─────────┼─────────┼─────────┼─────────┼─────────┼─────────┼─────────┤
│0.0600000│0.0600000│0.2600000│0.4600000│0.6600000│0.8600000│1.0600000│
├─────────┼─────────┼─────────┼─────────┼─────────┼─────────┼─────────┤
│0.0800000│0.0800000│0.2800000│0.4800000│0.6800000│0.8800000│1.0800000│
├─────────┼─────────┼─────────┼─────────┼─────────┼─────────┼─────────┤
│0.1000000│0.1000000│0.3000000│0.5000000│0.7000000│0.9000000│1.1000000│
└─────────┴─────────┴─────────┴─────────┴─────────┴─────────┴─────────┘
┌─────────┬─────────┬───────────────┬──────────────┐
│    h    │   tau   │diff with exact│diff with prev│
├─────────┼─────────┼───────────────┼──────────────┤
│0.2000000│0.0100000│ 1.8873791e-15 │              │
├─────────┼─────────┼───────────────┼──────────────┤
│0.1000000│0.0100000│ 5.5511151e-16 │9.9920072e-16 │
├─────────┼─────────┼───────────────┼──────────────┤
│0.0500000│0.0100000│ 4.9960036e-16 │5.5511151e-16 │
├─────────┼─────────┼───────────────┼──────────────┤
│0.0250000│0.0100000│ 1.1657342e-15 │2.6645353e-15 │
└─────────┴─────────┴───────────────┴──────────────┘
```

При использовании неявного метода при различных параметрах $h$ и $\tau$ было найдено практически точное решение (погрешность равна машинному нулю). При использовании же явного метода начиная с $h = 0.1$ начинает проявляться его неустойчивость, точность значительно падает, что ожидаемо:
$$
A = \max \{a(x, t) \mid 0 \leqslant x \leqslant 1,0 \leqslant t \leqslant T\} = 1
$$

При $h = 0.2$

$$
\nu = \frac{\tau}{h^2}=\frac{0.01}{0.04} = 0.25 \le0.5
$$

При $h = 0.1$

$$
\nu = \frac{\tau}{h^2}=\frac{0.01}{0.01} = 1 > 0.5
$$

При $h = 0.05$

$$
\nu = \frac{\tau}{h^2}=\frac{0.01}{0.0025} = 4 > 0.5
$$

При $h = 0.025$

$$
\nu = \frac{\tau}{h^2}=\frac{0.01}{0.000625} = 16 > 0.5
$$

Таким образом, условие устойчивости выполняется только для $h = 0.1$

#### $u = x^3 + t^3$

```
sigma = 0
┌─────────┬─────────┬─────────┬─────────┬─────────┬─────────┬─────────┐
│         │0.0000000│0.2000000│0.4000000│0.6000000│0.8000000│1.0000000│
├─────────┼─────────┼─────────┼─────────┼─────────┼─────────┼─────────┤
│0.0000000│0.0000000│0.0080000│0.0640000│0.2160000│0.5120000│1.0000000│
├─────────┼─────────┼─────────┼─────────┼─────────┼─────────┼─────────┤
│0.0200000│0.0000080│0.0081829│0.0643355│0.2164757│0.5125979│1.0938329│
├─────────┼─────────┼─────────┼─────────┼─────────┼─────────┼─────────┤
│0.0400000│0.0000640│0.0084300│0.0647426│0.2170218│0.5631727│1.1359879│
├─────────┼─────────┼─────────┼─────────┼─────────┼─────────┼─────────┤
│0.0600000│0.0002160│0.0087850│0.0652650│0.2440491│0.5861357│1.1551491│
├─────────┼─────────┼─────────┼─────────┼─────────┼─────────┼─────────┤
│0.0800000│0.0005120│0.0092938│0.0796434│0.2565933│0.6091158│1.1743485│
├─────────┼─────────┼─────────┼─────────┼─────────┼─────────┼─────────┤
│0.1000000│0.0010000│0.0169870│0.0866203│0.2757314│0.6254423│1.1880352│
└─────────┴─────────┴─────────┴─────────┴─────────┴─────────┴─────────┘
┌─────────┬─────────┬───────────────┬──────────────┐
│    h    │   tau   │diff with exact│diff with prev│
├─────────┼─────────┼───────────────┼──────────────┤
│0.2000000│0.0100000│ 6.3325599e-01 │              │
├─────────┼─────────┼───────────────┼──────────────┤
│0.1000000│0.0100000│ 8.0775629e-04 │3.8275293e-01 │
├─────────┼─────────┼───────────────┼──────────────┤
│0.0500000│0.0100000│ 4.7263927e-03 │4.7893309e-03 │
├─────────┼─────────┼───────────────┼──────────────┤
│0.0250000│0.0100000│ 1.6597811e+00 │1.6641438e+00 │
└─────────┴─────────┴───────────────┴──────────────┘
sigma = 1
┌─────────┬─────────┬─────────┬─────────┬─────────┬─────────┬─────────┐
│         │0.0000000│0.2000000│0.4000000│0.6000000│0.8000000│1.0000000│
├─────────┼─────────┼─────────┼─────────┼─────────┼─────────┼─────────┤
│0.0000000│0.0000000│0.0080000│0.0640000│0.2160000│0.5120000│1.0000000│
├─────────┼─────────┼─────────┼─────────┼─────────┼─────────┼─────────┤
│0.0200000│0.0000080│0.0088780│0.0670770│0.2263887│0.5476850│1.1230722│
├─────────┼─────────┼─────────┼─────────┼─────────┼─────────┼─────────┤
│0.0400000│0.0000640│0.0107936│0.0726735│0.2401014│0.5748032│1.1456800│
├─────────┼─────────┼─────────┼─────────┼─────────┼─────────┼─────────┤
│0.0600000│0.0002160│0.0137103│0.0799750│0.2545105│0.5966511│1.1639119│
├─────────┼─────────┼─────────┼─────────┼─────────┼─────────┼─────────┤
│0.0800000│0.0005120│0.0174400│0.0882697│0.2686164│0.6150549│1.1792978│
├─────────┼─────────┼─────────┼─────────┼─────────┼─────────┼─────────┤
│0.1000000│0.0010000│0.0217752│0.0970659│0.2820885│0.6310943│1.1927452│
└─────────┴─────────┴─────────┴─────────┴─────────┴─────────┴─────────┘
┌─────────┬─────────┬───────────────┬──────────────┐
│    h    │   tau   │diff with exact│diff with prev│
├─────────┼─────────┼───────────────┼──────────────┤
│0.2000000│0.0100000│ 6.7689625e-01 │              │
├─────────┼─────────┼───────────────┼──────────────┤
│0.1000000│0.0100000│ 1.7436078e-02 │3.8529182e-01 │
├─────────┼─────────┼───────────────┼──────────────┤
│0.0500000│0.0100000│ 1.4188748e-03 │1.0014035e-02 │
├─────────┼─────────┼───────────────┼──────────────┤
│0.0250000│0.0100000│ 2.6447315e-04 │7.4541473e-04 │
└─────────┴─────────┴───────────────┴──────────────┘
```

Еще больше проявляется неустойчивость явной схемы, в случае  $h = 0.025$ была получена погрешность, большая единицы. В случае с неявной схемой погрешность уменьшается, при этом не превышая $O(\tau + h)$ (в данном случае порядок аппроксимации таков, поскольку один из коэффициентов при производных в граничных условиях не равен нулю)
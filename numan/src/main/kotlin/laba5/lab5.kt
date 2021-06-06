package laba5

import com.jakewharton.picnic.TextAlignment
import com.jakewharton.picnic.table
import eFormat
import format
import get
import minus
import org.apache.commons.math3.linear.RealMatrix
import set
import toMatrix
import kotlin.math.pow
import kotlin.math.roundToInt
import kotlin.math.sin

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
        u = { x: Double, t: Double -> x.pow(2) + t.pow(2) },
        dudt = { x: Double, t: Double -> 2 * t },
        dudx = { x: Double, t: Double -> 2 * x },
        dudx2 = { x: Double, t: Double -> 2.0 },
        "x^2 + t^2"
    ),
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
    ),
    ExactSolution(
        u = { x: Double, t: Double -> x.pow(2) + t },
        dudt = { x: Double, t: Double -> 1.0 },
        dudx = { x: Double, t: Double -> 2 * x },
        dudx2 = { x: Double, t: Double -> 2.0 },
        "x^2 + t"
    ),
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

    listOf(0.2, 0.1, 0.05, 0.025, 0.0125).forEach { h ->
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
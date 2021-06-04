package laba4

import com.jakewharton.picnic.TextAlignment
import com.jakewharton.picnic.table
import eFormat
import get
import kronecker
import org.apache.commons.math3.linear.LUDecomposition
import toMatrix
import toVector
import kotlin.math.abs
import kotlin.math.cos
import kotlin.math.pow

fun main() {
    // Исходные данные
    val H = { x: Double, y: Double -> 0.6 * cos(x * y.pow(2)) }
    val f = { x: Double -> x - 0.6 }
    val a = 0.0
    val b = 1.0

    val epsilon = 0.000001
    var nodesNumber = 1
    var max = Double.POSITIVE_INFINITY
    val result = mutableListOf<NResult>()

    // Пока не выполняется условие выхода, считаем приближенное решение уравнения с текущим количеством шагов,
    // в конце каждой итерации удваиваем его
    while (max >= epsilon) {
        val (u) = solveFredholm(nodesNumber, a, b, H, f)
        var nResult = NResult(
            nodesNumber,
            u(a),
            u((a + b) / 2),
            u(b),
            u,
            null
        )

        if (result.isNotEmpty()) {
            max = maxOf(
                abs(nResult.ua - result.last().ua),
                abs(nResult.um - result.last().um),
                abs(nResult.ub - result.last().ub)
            )
            nResult = nResult.copy(max = max)
        }

        result.add(nResult)
        nodesNumber *= 2
    }

    // Вывод результатов
    table {
        cellStyle {
            border = true
            alignment = TextAlignment.TopCenter
        }
        header { row("nodes", "u(a)","u(a + b / 2)", "u(b)", "max | u^2mn - u^mn |") }
        result.forEach {
            row(it.nodesNumber, it.ua.eFormat(), it.um.eFormat(), it.ub.eFormat(), it.max?.eFormat() ?: "--")
        }
    }.also { println(it) }
}

/**
 * Вспомогательный класс для хранения результатов итерации
 */
data class NResult(val nodesNumber: Int,
                   val ua: Double,
                   val um: Double,
                   val ub: Double,
                   val u: (Double) -> Double,
                   val max: Double?)

/**
 * Квардратурная формула средних прямоугольников
 */
fun rectangleMethod(nodesNumber: Int, a: Double, b: Double): IntegrationResult {
    require(b >= a)
    val h = (b - a) / nodesNumber
    val nodes = (1..nodesNumber).map { a + h / 2.0 + (it - 1) * h }
    return IntegrationResult(h, nodes)
}

data class IntegrationResult(val h: Double, val nodes: List<Double>)

fun solveFredholm(nodesNumber: Int,
                  a: Double,
                  b: Double,
                  H: (Double, Double) -> Double,
                  f: (Double) -> Double
): FredholmResult {
    val (h, nodes) = rectangleMethod(nodesNumber, a, b)
    val systemMatrix = Array(nodesNumber) { DoubleArray(nodesNumber) { 0.0 } }

    // Заполняем матрицу D
    for(j in 0 until nodesNumber) {
        for(k in 0 until nodesNumber) {
            systemMatrix[j][k] = kronecker(j, k) - h * H(nodes[j], nodes[k])
        }
    }
    val constVector = nodes.map { f(it) }.toDoubleArray().toVector()
    val solver = LUDecomposition(systemMatrix.toMatrix()).solver
    val solution = solver.solve(constVector)
    return FredholmResult { x: Double ->
        // Решение
        f(x) + h * nodes.mapIndexed { i, xi -> H(x, xi) * solution[i] }.sum()
    }
}

data class FredholmResult(val u: (Double) -> Double)
package laba5

import org.apache.commons.math3.linear.RealVector

fun main() {

}

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

fun solveTridiagonalSystem(system: TridiagonalSystem): RealVector {
    val coefficients = mutableListOf(
        TridiagonalSolutionCoefficients(
            s = system.C0 / system.B0,
            t =  -system.G0 / system.B0
        )
    )

    for(i in system.rows.indices) {

    }
}
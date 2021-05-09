import com.jakewharton.picnic.Table
import com.jakewharton.picnic.TextAlignment
import com.jakewharton.picnic.table
import org.apache.commons.math3.linear.RealMatrix
import org.apache.commons.math3.linear.RealVector

fun RealMatrix.toTable(): Table {
    val matrix = this
    return table {
        cellStyle {
            border = true
            alignment = TextAlignment.TopCenter
        }
        for(i in 0 until matrix.rowDimension) {
            row {
                for(j in 0 until matrix.columnDimension) {
                    cell(matrix[i, j].format())
                }
            }
        }
    }
}

fun RealVector.toTable(): Table {
    val vector = this
    return table {
        cellStyle {
            border = true
            alignment = TextAlignment.TopCenter
        }
        row {
            for(i in 0 until vector.dimension) {
                cell(vector[i].format())
            }
        }
    }
}

fun Double.format(precision: Int = 7): String = String.format("%.${precision}f", this)
fun Double.eFormat(precision: Int = 7): String = String.format("%.${precision}e", this)
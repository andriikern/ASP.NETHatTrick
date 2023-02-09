const addToDate =
  function (
    date: Date | null | undefined,
    milliseconds: number | null | undefined
  ): Date | null | undefined {
    if (date === undefined || date === null)
      return date
    if (milliseconds === undefined || milliseconds === null)
      return milliseconds

    return new Date(date.getTime() + milliseconds)
}

const dateToISOStringWithOffset =
  function (date: Date | null | undefined): string | null | undefined {
    if (date === undefined || date === null)
      return date

    const offset = date.getTimezoneOffset()

    const offsetSign = Math.sign(offset) === -1 ? '-' : '+'
    const offsetAbs = Math.abs(offset)

    const offsetAbsHours = Math.floor(offsetAbs / 60)
    const offsetAbsMinutes = offsetAbs % 60

    let isoString = new Date(date.getTime() - offset * 60_000).toISOString()
    isoString =
      isoString.slice(0, -1) +
        offsetSign +
        offsetAbsHours.toString().padStart(2, '0') +
        ':' +
        offsetAbsMinutes.toString().padStart(2, '0')

    return isoString
  }

export {
  addToDate,
  dateToISOStringWithOffset
}

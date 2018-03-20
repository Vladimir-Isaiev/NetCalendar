jQuery.extend(jQuery.validator.methods, {
    date: function (value, element) {
        return this.optional(element) || /^\d\d?\.\d\d?\.\d\d\d?\d?$/.test(value);
    },
    number: function (value, element) {
        return this.optional(element) || /^-?(?:\d+|\d{1,3}(?: \d{3})+)(?:,\d+)?$/.test(value);
    }
});

//original:
// http://jqueryvalidation.org/date-method/
//date: function( value, element ) {
//    return this.optional( element ) || !/Invalid|NaN/.test( new Date( value ).toString() );
//http://jqueryvalidation.org/dateISO-method/
//dateISO: function( value, element ) {
//    return this.optional( element ) || /^\d{4}[\/\-](0?[1-9]|1[012])[\/\-](0?[1-9]|[12][0-9]|3[01])$/.test( value );

//http://jqueryvalidation.org/number-method/
//number: function( value, element ) {
//    return this.optional( element ) || /^(?:-?\d+|-?\d{1,3}(?:,\d{3})+)?(?:\.\d+)?$/.test( value );
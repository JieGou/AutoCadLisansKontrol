﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
namespace AutoCadLisansKontrol.DAL
{
    public class Converter
    {
        public static TOut Convert<TOut, TIn>(TIn fromRecord) where TOut : new()
        {
            if (fromRecord == null) return new TOut();
            var toRecord = new TOut();
            PropertyInfo[] fromFields = null;
            PropertyInfo[] toFields = null;

            fromFields = typeof(TIn).GetProperties();
            toFields = typeof(TOut).GetProperties();

            foreach (var fromField in fromFields)
            {
                foreach (var toField in toFields)
                {
                    var totype = toField.PropertyType;
                    var fromtype = fromField.PropertyType;
                    if (fromField.Name == toField.Name&& fromField.PropertyType==toField.PropertyType)
                    {
                        //if (fromField.PropertyType.IsPrimitive)
                            toField.SetValue(toRecord, fromField.GetValue(fromRecord, null), null);
                        break;
                    }

                }

            }
            return toRecord;
        }

        public static List<TOut> Convert<TOut, TIn>(List<TIn> fromRecordList) where TOut : new()
        {
            return fromRecordList.Count == 0 ? new List<TOut>() : fromRecordList.Select(Convert<TOut, TIn>).ToList();
        }
    }
}

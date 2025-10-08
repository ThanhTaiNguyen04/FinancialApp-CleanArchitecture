import React from 'react';
import { View, Text, StyleSheet } from 'react-native';
import { Colors } from '../../utils/colors';

const CompassLogo = ({ size = 120 }) => {
  const logoSize = size;
  const outerCircleSize = logoSize;
  const middleCircleSize = logoSize * 0.67;
  const innerCircleSize = logoSize * 0.4;
  const pointSize = logoSize * 0.13;

  return (
    <View style={[styles.container, { width: logoSize, height: logoSize }]}>
      {/* Outer Circle */}
      <View style={[
        styles.outerCircle, 
        { width: outerCircleSize, height: outerCircleSize, borderRadius: outerCircleSize / 2 }
      ]}>
        {/* Middle Circle */}
        <View style={[
          styles.middleCircle,
          { width: middleCircleSize, height: middleCircleSize, borderRadius: middleCircleSize / 2 }
        ]}>
          {/* Inner Circle */}
          <View style={[
            styles.innerCircle,
            { width: innerCircleSize, height: innerCircleSize, borderRadius: innerCircleSize / 2 }
          ]}>
            <Text style={[styles.dollarSign, { fontSize: logoSize * 0.2 }]}>$</Text>
          </View>
        </View>
      </View>
      
      {/* Compass Points */}
      <View style={[styles.compassPoint, styles.pointTop, { 
        top: -pointSize * 0.5, 
        left: '50%', 
        marginLeft: -pointSize * 0.5,
        borderLeftWidth: pointSize * 0.5,
        borderRightWidth: pointSize * 0.5,
        borderBottomWidth: pointSize * 1.2,
      }]} />
      
      <View style={[styles.compassPoint, styles.pointRight, { 
        right: -pointSize * 0.5,
        top: '50%', 
        marginTop: -pointSize * 0.5,
        borderTopWidth: pointSize * 0.5,
        borderBottomWidth: pointSize * 0.5,
        borderLeftWidth: pointSize * 1.2,
      }]} />
      
      <View style={[styles.compassPoint, styles.pointBottom, { 
        bottom: -pointSize * 0.5,
        left: '50%', 
        marginLeft: -pointSize * 0.5,
        borderLeftWidth: pointSize * 0.5,
        borderRightWidth: pointSize * 0.5,
        borderTopWidth: pointSize * 1.2,
      }]} />
      
      <View style={[styles.compassPoint, styles.pointLeft, { 
        left: -pointSize * 0.5,
        top: '50%', 
        marginTop: -pointSize * 0.5,
        borderTopWidth: pointSize * 0.5,
        borderBottomWidth: pointSize * 0.5,
        borderRightWidth: pointSize * 1.2,
      }]} />
    </View>
  );
};

const styles = StyleSheet.create({
  container: {
    position: 'relative',
    justifyContent: 'center',
    alignItems: 'center',
  },
  outerCircle: {
    backgroundColor: 'rgba(255,255,255,0.25)',
    justifyContent: 'center',
    alignItems: 'center',
    position: 'relative',
  },
  middleCircle: {
    backgroundColor: 'rgba(255,255,255,0.4)',
    justifyContent: 'center',
    alignItems: 'center',
  },
  innerCircle: {
    backgroundColor: Colors.white,
    justifyContent: 'center',
    alignItems: 'center',
    elevation: 3,
    shadowColor: Colors.shadowColor,
    shadowOffset: { width: 0, height: 2 },
    shadowOpacity: 0.2,
    shadowRadius: 4,
  },
  dollarSign: {
    fontWeight: 'bold',
    color: Colors.primaryDark,
  },
  compassPoint: {
    position: 'absolute',
    width: 0,
    height: 0,
  },
  pointTop: {
    borderLeftColor: 'transparent',
    borderRightColor: 'transparent',
    borderBottomColor: Colors.white,
  },
  pointRight: {
    borderTopColor: 'transparent',
    borderBottomColor: 'transparent',
    borderLeftColor: Colors.white,
  },
  pointBottom: {
    borderLeftColor: 'transparent',
    borderRightColor: 'transparent',
    borderTopColor: Colors.white,
  },
  pointLeft: {
    borderTopColor: 'transparent',
    borderBottomColor: 'transparent',
    borderRightColor: Colors.white,
  },
});

export default CompassLogo;
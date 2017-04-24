/*
 * Copyright 2016 Xamarin Inc.
 *
 * Use of this source code is governed by a BSD-style license that can be
 * found in the LICENSE file.
 */

#include "SkPixmap.h"
#include "SkBitmapScaler.h"
#include "SkImageEncoder.h"

#include "sk_pixmap.h"

#include "sk_types_priv.h"


void sk_pixmap_destructor(sk_pixmap_t* cpixmap)
{
    delete AsPixmap(cpixmap);
}

sk_pixmap_t* sk_pixmap_new()
{
    return ToPixmap(new SkPixmap());
}

sk_pixmap_t* sk_pixmap_new_with_params(const sk_imageinfo_t* cinfo, const void* addr, size_t rowBytes, sk_colortable_t* ctable)
{
    SkImageInfo info;
    from_c(*cinfo, &info);

    return ToPixmap(new SkPixmap(info, addr, rowBytes, AsColorTable(ctable)));
}

void sk_pixmap_reset(sk_pixmap_t* cpixmap)
{
    AsPixmap(cpixmap)->reset();
}

void sk_pixmap_reset_with_params(sk_pixmap_t* cpixmap, const sk_imageinfo_t* cinfo, const void* addr, size_t rowBytes, sk_colortable_t* ctable)
{
    SkImageInfo info;
    from_c(*cinfo, &info);

    AsPixmap(cpixmap)->reset(info, addr, rowBytes, AsColorTable(ctable));
}

void sk_pixmap_get_info(sk_pixmap_t* cpixmap, sk_imageinfo_t* cinfo)
{
    from_sk(AsPixmap(cpixmap)->info(), cinfo);
}

size_t sk_pixmap_get_row_bytes(sk_pixmap_t* cpixmap)
{
    return AsPixmap(cpixmap)->rowBytes();
}

const void* sk_pixmap_get_pixels(sk_pixmap_t* cpixmap)
{
    return AsPixmap(cpixmap)->addr();
}

sk_colortable_t* sk_pixmap_get_colortable(sk_pixmap_t* cpixmap)
{
    return ToColorTable(AsPixmap(cpixmap)->ctable());
}

bool sk_bitmapscaler_resize(const sk_pixmap_t* cdst, const sk_pixmap_t* csrc, sk_bitmapscaler_resizemethod_t method)
{
    const SkPixmap& dst = AsPixmap(*cdst);
    const SkPixmap& src = AsPixmap(*csrc);
    return SkBitmapScaler::Resize(dst, src, (SkBitmapScaler::ResizeMethod)method);
}

sk_color_t sk_color_unpremultiply(const sk_pmcolor_t pmcolor)
{
    return SkUnPreMultiply::PMColorToColor(pmcolor);
}

sk_pmcolor_t sk_color_premultiply(const sk_color_t color)
{
    return SkPreMultiplyColor(color);
}

void sk_color_unpremultiply_array(const sk_pmcolor_t* pmcolors, int size, sk_color_t* colors)
{
    for (int i = 0; i < size; ++i) {
        colors[i] = SkUnPreMultiply::PMColorToColor(pmcolors[i]);
    }
}

void sk_color_premultiply_array(const sk_color_t* colors, int size, sk_pmcolor_t* pmcolors)
{
    for (int i = 0; i < size; ++i) {
        pmcolors[i] = SkPreMultiplyColor(colors[i]);
    }
}

void sk_color_get_bit_shift(int* a, int* r, int* g, int* b)
{
    *a = (int)SK_A32_SHIFT;
    *r = (int)SK_R32_SHIFT;
    *g = (int)SK_G32_SHIFT;
    *b = (int)SK_B32_SHIFT;
}

bool sk_pixmap_encode_image(sk_wstream_t* dst, const sk_pixmap_t* src, sk_encoded_image_format_t encoder, int quality)
{
    return SkEncodeImage(AsWStream(dst), AsPixmap(*src), (SkEncodedImageFormat)encoder, quality);
}
